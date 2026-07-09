import { useState } from 'react'
import { AuthLogic } from '@graft/nuget-authservice'
import type { CityWeather } from '@graft/nuget-cityweatherservice'
import { WeatherLogic } from '@graft/nuget-cityweatherservice'
import { AuthGraftConfig, WeatherGraftConfig } from './graft/setup'
import './App.css'

function authHeaders(token: string) {
  return { Authorization: `Bearer ${token}` }
}

function App() {
  const [username, setUsername] = useState('wad')
  const [password, setPassword] = useState('password')
  const [token, setToken] = useState<string | null>(null)
  const [loggedInUser, setLoggedInUser] = useState<string | null>(null)
  const [cities, setCities] = useState<string[]>([])
  const [selectedCity, setSelectedCity] = useState<string | null>(null)
  const [weather, setWeather] = useState<CityWeather | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function handleLogin(event: React.FormEvent) {
    event.preventDefault()
    setLoading(true)
    setError(null)
    setWeather(null)
    setSelectedCity(null)

    try {
      const result = await AuthLogic.login(username, password)
      const issuedToken = result.get_token() as string
      const user = result.get_username() as string

      setToken(issuedToken)
      setLoggedInUser(user)

      const favouriteCities = await AuthGraftConfig.invokeWithHeaders(
        () => AuthLogic.getFavouriteCities(),
        authHeaders(issuedToken),
      )
      setCities(favouriteCities)
    } catch (err) {
      setToken(null)
      setLoggedInUser(null)
      setCities([])
      setError(err instanceof Error ? err.message : String(err))
    } finally {
      setLoading(false)
    }
  }

  async function handleSelectCity(city: string) {
    if (!token) return

    setSelectedCity(city)
    setLoading(true)
    setError(null)

    try {
      const result = await WeatherGraftConfig.invokeWithHeaders(
        () => WeatherLogic.getWeather(city),
        authHeaders(token),
      )
      setWeather(result)
    } catch (err) {
      setWeather(null)
      setError(err instanceof Error ? err.message : String(err))
    } finally {
      setLoading(false)
    }
  }

  function handleLogout() {
    setToken(null)
    setLoggedInUser(null)
    setCities([])
    setSelectedCity(null)
    setWeather(null)
    setError(null)
  }

  return (
    <div className="app">
      <header className="header">
        <div>
          <h1>City Weather</h1>
          <p className="subtitle">WeAreDevelopers 2026 — Graftcode workshop</p>
        </div>
        {loggedInUser && (
          <div className="user-bar">
            <span>Signed in as <strong>{loggedInUser}</strong></span>
            <button type="button" className="secondary" onClick={handleLogout}>
              Log out
            </button>
          </div>
        )}
      </header>

      {!token ? (
        <section className="card login-card">
          <h2>Log in</h2>
          <form onSubmit={handleLogin} className="login-form">
            <label>
              Username
              <input
                value={username}
                onChange={(event) => setUsername(event.target.value)}
                autoComplete="username"
              />
            </label>
            <label>
              Password
              <input
                type="password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                autoComplete="current-password"
              />
            </label>
            <button type="submit" disabled={loading}>
              {loading ? 'Signing in…' : 'Sign in'}
            </button>
          </form>
          <p className="hint">Demo user: <code>wad</code> / <code>password</code></p>
        </section>
      ) : (
        <div className="layout">
          <section className="card">
            <h2>Favourite cities</h2>
            {cities.length === 0 ? (
              <p className="muted">No favourite cities yet.</p>
            ) : (
              <ul className="city-list">
                {cities.map((city) => (
                  <li key={city}>
                    <button
                      type="button"
                      className={selectedCity === city ? 'city active' : 'city'}
                      onClick={() => handleSelectCity(city)}
                      disabled={loading}
                    >
                      {city}
                    </button>
                  </li>
                ))}
              </ul>
            )}
          </section>

          <section className="card weather-card">
            <h2>Weather</h2>
            {!selectedCity ? (
              <p className="muted">Select a city to see the forecast.</p>
            ) : !weather ? (
              <p className="muted">{loading ? 'Loading weather…' : 'No weather data.'}</p>
            ) : (
              <div className="weather">
                <div className="weather-main">
                  <h3>
                    {weather.get_city() as string}, {weather.get_country() as string}
                  </h3>
                  <p className="temperature">{weather.get_temperature() as number}°C</p>
                  <p className="condition">{weather.get_condition() as string}</p>
                </div>
                <dl className="weather-details">
                  <div>
                    <dt>Humidity</dt>
                    <dd>{weather.get_humidity() as number}%</dd>
                  </div>
                  <div>
                    <dt>Wind</dt>
                    <dd>{weather.get_wind() as number} km/h</dd>
                  </div>
                  <div>
                    <dt>Last updated</dt>
                    <dd>{weather.get_lastUpdated() as string}</dd>
                  </div>
                </dl>
              </div>
            )}
          </section>
        </div>
      )}

      {error && <p className="error" role="alert">{error}</p>}
    </div>
  )
}

export default App
