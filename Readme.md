How does react work with Razor Pages
* Razor Pages will serve as the initial entry point (just the shell/container)
* React will handle all client-side UI rendering and navigation
* Your ASP.NET Core backend will provide API endpoints for data


How They Work Together:

* Index.cshtml becomes just a container with a <div id="react-app"></div>
* The React application loads in this div and takes over the UI
* The Razor Pages routing only handles the initial page load
* React Router handles all client-side navigation after that


Must install:
* npm
* npm install react react-dom react-router-dom