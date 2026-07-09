import { GraftConfig as AuthGraftConfig } from '@graft/nuget-authservice'
import { GraftConfig as WeatherGraftConfig } from '@graft/nuget-cityweatherservice'

const origin = typeof window !== 'undefined' ? window.location.origin : ''

AuthGraftConfig.host = `${origin}/auth/h2`
AuthGraftConfig.stateless = true

WeatherGraftConfig.host = `${origin}/weather/h2`
WeatherGraftConfig.stateless = true

export { AuthGraftConfig, WeatherGraftConfig }
