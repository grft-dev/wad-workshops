import path from 'node:path'
import http2 from 'node:http2'
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import basicSsl from '@vitejs/plugin-basic-ssl'
import { nodePolyfills } from 'vite-plugin-node-polyfills'

const H2C_UPSTREAMS: Record<string, string> = {
  '/auth/h2': 'http://localhost:8989',
  '/weather/h2': 'http://localhost:8990',
}

const FORBIDDEN_H2_REQUEST_HEADERS = new Set([
  'connection',
  'keep-alive',
  'proxy-connection',
  'transfer-encoding',
  'upgrade',
  'host',
  'http2-settings',
])

function buildH2RequestHeaders(req: { method?: string; url?: string; headers: Record<string, unknown> }, alias: string) {
  let remainder = (req.url ?? '').slice(alias.length)
  if (remainder === '/') remainder = ''
  return {
    ':method': req.method || 'GET',
    ':path': `/h2${remainder}`,
    ...Object.fromEntries(
      Object.entries(req.headers)
        .filter(([k]) => !FORBIDDEN_H2_REQUEST_HEADERS.has(k.toLowerCase()) && !k.startsWith(':'))
        .map(([k, v]) => [k.toLowerCase(), v]),
    ),
  }
}

function buildHttp1ResponseHeaders(h2Headers: Record<string, unknown>) {
  return Object.fromEntries(Object.entries(h2Headers).filter(([k]) => !k.startsWith(':')))
}

function h2cProxy() {
  return {
    name: 'graft-h2c-proxy',
    configureServer(server: { middlewares: { use: (fn: (req: any, res: any, next: () => void) => void) => void } }) {
      server.middlewares.use((req, res, next) => {
        const alias = Object.keys(H2C_UPSTREAMS).find((a) => req.url?.startsWith(a))
        if (!alias) return next()

        const client = http2.connect(H2C_UPSTREAMS[alias])
        const upstream = client.request(buildH2RequestHeaders(req, alias))
        req.pipe(upstream)
        upstream.on('response', (headers) => {
          res.writeHead((headers[':status'] as number) || 200, buildHttp1ResponseHeaders(headers))
          upstream.pipe(res)
        })
        upstream.on('error', (error) => {
          res.statusCode = 502
          res.end(String(error))
          client.close()
        })
        res.on('close', () => client.close())
      })
    },
  }
}

export default defineConfig({
  plugins: [react(), basicSsl(), nodePolyfills(), h2cProxy()],
  resolve: {
    alias: {
      crypto: path.resolve(__dirname, 'src/crypto-shim.ts'),
    },
  },
  server: {
    proxy: {},
    strictPort: true,
    port: 5173,
  },
})
