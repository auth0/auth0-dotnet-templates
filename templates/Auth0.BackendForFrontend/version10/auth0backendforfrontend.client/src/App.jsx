import { useEffect, useState } from 'react';
import './App.css';
import { useAuth } from './AuthContext';
import LoginButton from './LoginButton';
import LogoutButton from './LogoutButton';
import Profile from './Profile';

function App() {
    const [forecasts, setForecasts] = useState();
    const { isAuthenticated, login, logout } = useAuth();

    useEffect(() => {
        populateWeatherData();
    }, []);

    const contents = forecasts === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <table className="table table-striped" aria-labelledby="tableLabel">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temp. (C)</th>
                    <th>Temp. (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
            <tbody>
                {forecasts.map(forecast =>
                    <tr key={forecast.date}>
                        <td>{forecast.date}</td>
                        <td>{forecast.temperatureC}</td>
                        <td>{forecast.temperatureF}</td>
                        <td>{forecast.summary}</td>
                    </tr>
                )}
            </tbody>
        </table>;

    return (
        <div>
            <h1 id="tableLabel">Weather forecast</h1>
            <p>This component demonstrates fetching data from the server.</p>

            <hr/>

            {isAuthenticated ? (
              <div>
                <div>
                  <Profile />
                </div>

                <hr />

                <h2>Weather data</h2>
                <div>
                {contents}
                </div>

                <hr />

                <LogoutButton />
              </div>
            ) : (
              <div className="action-card">
                <p className="action-text">Get started by signing in to your account</p>
                <LoginButton />
              </div>
            )}
        </div>
    );
    
    async function populateWeatherData() {
        const response = await fetch('/bff/weatherforecast');
        if (response.ok) {
            const data = await response.json();
            setForecasts(data);
        }
    }
}

export default App;