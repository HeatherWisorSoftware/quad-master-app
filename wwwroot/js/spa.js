/**
 * SPA Navigation System
 * This file contains all the JavaScript needed for the Single Page Application functionality.
 */

// Initialize the SPA when the document is fully loaded
document.addEventListener('DOMContentLoaded', function () {
    // Add click event listeners to navigation links
    const navLinks = document.querySelectorAll('a[data-page]');
    navLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const page = this.getAttribute('data-page');
            navigateTo(page);
        });
    });

    // Add click event listeners for buttons with data-page attribute
    document.querySelectorAll('button[data-page]').forEach(button => {
        button.addEventListener('click', function () {
            const page = this.getAttribute('data-page');
            navigateTo(page);
        });
    });

    // Initialize the SPA - load page from URL or default to dashboard
    initializeSPA();

    // Handle browser back/forward buttons
    window.addEventListener('popstate', function (event) {
        if (event.state && event.state.page) {
            loadPage(event.state.page);
        } else {
            loadPage('dashboard');
        }
    });
});

/**
 * Initialize the SPA
 * Checks URL for the page to load and sets initial browser history state
 */
function initializeSPA() {
    // Check URL for page parameter
    const hash = window.location.hash.substring(1) || 'dashboard';
    loadPage(hash);

    // Set initial state for browser history
    history.replaceState({ page: hash }, hash, `#${hash}`);
}

/**
 * Navigate to a specific page
 * @param {string} page - The page identifier to navigate to
 */
function navigateTo(page) {
    loadPage(page);
    history.pushState({ page: page }, page, `#${page}`);
}

/**
 * Load page content from template
 * @param {string} page - The page identifier to load
 */
function loadPage(page) {
    // Get the page content container
    const contentContainer = document.getElementById('page-content');

    // Get the template for the requested page
    const template = document.getElementById(`${page}-template`);

    if (template) {
        // Clear the current content
        contentContainer.innerHTML = '';

        // Clone the template content and append it to the container
        const content = template.content.cloneNode(true);
        contentContainer.appendChild(content);

        // Update active navigation link
        updateActiveNavLink(page);

        // If there are any page-specific initialization functions, call them
        if (typeof window[`initialize${page.charAt(0).toUpperCase() + page.slice(1)}Page`] === 'function') {
            window[`initialize${page.charAt(0).toUpperCase() + page.slice(1)}Page`]();
        }
    } else {
        console.error(`Template not found for page: ${page}`);
        // Load dashboard page as fallback
        if (page !== 'dashboard') {
            loadPage('dashboard');
        }
    }
}

/**
 * Update active navigation link
 * @param {string} page - The current page
 */
function updateActiveNavLink(page) {
    const navLinks = document.querySelectorAll('.nav-link');
    navLinks.forEach(link => {
        if (link.getAttribute('data-page') === page) {
            link.classList.add('active');
        } else {
            link.classList.remove('active');
        }
    });
}

/**
 * Fetch data from the server using AJAX
 * @param {string} endpoint - API endpoint
 * @param {Function} callback - Callback function to process the data
 */
function fetchData(endpoint, callback) {
    fetch(endpoint)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            callback(data);
        })
        .catch(error => {
            console.error('Fetch error:', error);
        });
}

/**
 * Initialize dashboard page
 * Fetches data from the API and populates the dashboard
 */
function initializeDashboardPage() {
    console.log('Dashboard page initialized');

    // Fetch dashboard data from the API
    fetchData('/api/main/dashboard', function (data) {
        // Show/hide welcome message based on whether tournaments exist
        const dashboardWelcome = document.getElementById('dashboard-welcome');
        if (data.hasTournaments) {
            dashboardWelcome.innerHTML = `
                <p>Welcome Back</p>
                <div style="display: flex; gap: 10px;">
                    <button type="button" class="btn btn-primary" onclick="navigateTo('tournaments')">Manage Tournaments</button>
                    <button type="button" class="btn btn-primary" onclick="navigateTo('create-tournament')">Create New Tournament</button>
                </div>
            `;
        } else {
            dashboardWelcome.innerHTML = `
                <p>Let's set up your first tournament</p>
                <button type="button" class="btn btn-primary" onclick="navigateTo('create-tournament')">Create New Tournament</button>
            `;
        }

        // Show/hide tournament table based on whether tournaments exist
        if (data.latestTournaments && data.latestTournaments.length > 0) {
            document.getElementById('no-tournaments-message').style.display = 'none';
            document.getElementById('tournaments-table-container').style.display = 'block';

            // Populate the tournaments table
            const tableBody = document.getElementById('latest-tournaments-tbody');
            tableBody.innerHTML = '';

            data.latestTournaments.forEach(tournament => {
                const row = document.createElement('tr');

                // Format the date
                const tournamentDate = new Date(tournament.date);
                const formattedDate = tournamentDate.toLocaleDateString();

                row.innerHTML = `
                    <td>${tournament.id}</td>
                    <td>${tournament.name}</td>
                    <td>${formattedDate}</td>
                    <td>${tournament.playerCount}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary view-tournament" data-id="${tournament.id}">View</button>
                    </td>
                `;

                tableBody.appendChild(row);
            });

            // Add event listeners to view buttons
            document.querySelectorAll('.view-tournament').forEach(button => {
                button.addEventListener('click', function () {
                    const tournamentId = this.getAttribute('data-id');
                    // Navigate to tournament details page (implement this later)
                    console.log(`Viewing tournament: ${tournamentId}`);
                });
            });
        } else {
            document.getElementById('no-tournaments-message').style.display = 'block';
            document.getElementById('tournaments-table-container').style.display = 'none';
        }
    });

    // Add event listener for the test database button
    const testDbButton = document.getElementById('test-database-btn');
    if (testDbButton) {
        testDbButton.addEventListener('click', function () {
            this.disabled = true;
            this.textContent = 'Testing...';

            fetch('/api/main/test-database', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    const resultElement = document.getElementById('test-result');
                    resultElement.style.display = 'block';

                    if (data.success) {
                        resultElement.innerHTML = data.message;
                        resultElement.className = 'alert alert-success mb-4';

                        // Refresh the dashboard to show the new tournament
                        setTimeout(() => {
                            initializeDashboardPage();
                        }, 1000);
                    } else {
                        resultElement.innerHTML = data.error;
                        resultElement.className = 'alert alert-danger mb-4';
                    }

                    // Re-enable the button
                    this.disabled = false;
                    this.textContent = 'Test Database Access';
                })
                .catch(error => {
                    console.error('Error:', error);
                    const resultElement = document.getElementById('test-result');
                    resultElement.style.display = 'block';
                    resultElement.innerHTML = 'An error occurred while testing the database.';
                    resultElement.className = 'alert alert-danger mb-4';

                    // Re-enable the button
                    this.disabled = false;
                    this.textContent = 'Test Database Access';
                });
        });
    }
}

/**
 * Initialize tournaments page
 * Fetches all tournaments from the API
 */
function initializeTournamentsPage() {
    console.log('Tournaments page initialized');

    fetchData('/api/main/tournaments', function (data) {
        if (data && data.length > 0) {
            document.getElementById('no-all-tournaments-message').style.display = 'none';
            document.getElementById('all-tournaments-table-container').style.display = 'block';

            const tableBody = document.getElementById('all-tournaments-tbody');
            tableBody.innerHTML = '';

            data.forEach(tournament => {
                const row = document.createElement('tr');

                // Format the date
                const tournamentDate = new Date(tournament.date);
                const formattedDate = tournamentDate.toLocaleDateString();

                row.innerHTML = `
                    <td>${tournament.id}</td>
                    <td>${tournament.name}</td>
                    <td>${formattedDate}</td>
                    <td>${tournament.venueName}</td>
                    <td>${tournament.city}, ${tournament.state}</td>
                    <td>${tournament.playerCount}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary view-tournament-details" data-id="${tournament.id}">View</button>
                        <button class="btn btn-sm btn-outline-success edit-tournament" data-id="${tournament.id}">Edit</button>
                        <button class="btn btn-sm btn-outline-danger delete-tournament" data-id="${tournament.id}">Delete</button>
                    </td>
                `;

                tableBody.appendChild(row);
            });

            // Add event listeners to the buttons
            // (implement these functions later)
        } else {
            document.getElementById('no-all-tournaments-message').style.display = 'block';
            document.getElementById('all-tournaments-table-container').style.display = 'none';
        }
    });
}

/**
 * Initialize players page
 * Fetches all players from the API
 */
function initializePlayersPage() {
    console.log('Players page initialized');

    fetchData('/api/main/players', function (data) {
        if (data && data.length > 0) {
            document.getElementById('no-players-message').style.display = 'none';
            document.getElementById('players-table-container').style.display = 'block';

            const tableBody = document.getElementById('players-tbody');
            tableBody.innerHTML = '';

            data.forEach(player => {
                const row = document.createElement('tr');

                row.innerHTML = `
                    <td>${player.id}</td>
                    <td>${player.name}</td>
                    <td>${player.rating}</td>
                    <td>${player.tournamentName || '-'}</td>
                    <td>${player.quadName || '-'}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary edit-player" data-id="${player.id}">Edit</button>
                        <button class="btn btn-sm btn-outline-danger delete-player" data-id="${player.id}">Delete</button>
                    </td>
                `;

                tableBody.appendChild(row);
            });

            // Add event listeners to the buttons
            // (implement these functions later)
        } else {
            document.getElementById('no-players-message').style.display = 'block';
            document.getElementById('players-table-container').style.display = 'none';
        }
    });

    // Add event listener for the add player button
    const addPlayerBtn = document.getElementById('add-player-btn');
    if (addPlayerBtn) {
        addPlayerBtn.addEventListener('click', function () {
            // Implement player addition functionality
            console.log('Adding new player');
        });
    }
}

/**
 * Initialize quads page
 * Fetches all quads from the API
 */
function initializeQuadsPage() {
    console.log('Quads page initialized');

    fetchData('/api/main/quads', function (data) {
        if (data && data.length > 0) {
            document.getElementById('no-quads-message').style.display = 'none';

            const quadsContainer = document.getElementById('quads-container');
            quadsContainer.innerHTML = '';

            data.forEach(quad => {
                const quadCard = document.createElement('div');
                quadCard.className = 'col-md-6 mb-4';

                let playersHtml = '';
                if (quad.players && quad.players.length > 0) {
                    playersHtml = `
                        <table class="table table-sm">
                            <thead>
                                <tr>
                                    <th>Pos</th>
                                    <th>Name</th>
                                    <th>Rating</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                    `;

                    quad.players.forEach((player, index) => {
                        playersHtml += `
                            <tr>
                                <td>${index + 1}</td>
                                <td>${player.name}</td>
                                <td>${player.rating}</td>
                                <td>
                                    <button class="btn btn-sm btn-outline-danger remove-from-quad" data-player-id="${player.id}" data-quad-id="${quad.id}">Remove</button>
                                </td>
                            </tr>
                        `;
                    });

                    // Add empty slots if needed
                    const emptySlots = 4 - quad.players.length; // Assuming 4 players per quad
                    for (let i = 0; i < emptySlots; i++) {
                        playersHtml += `
                            <tr class="empty-slot">
                                <td>${quad.players.length + i + 1}</td>
                                <td colspan="2">Empty Slot</td>
                                <td>
                                    <button class="btn btn-sm btn-outline-success assign-to-quad" data-quad-id="${quad.id}">Assign</button>
                                </td>
                            </tr>
                        `;
                    }

                    playersHtml += `
                            </tbody>
                        </table>
                    `;
                } else {
                    playersHtml = `
                        <p>No players assigned to this quad.</p>
                        <button class="btn btn-sm btn-outline-primary assign-players" data-quad-id="${quad.id}">Assign Players</button>
                    `;
                }

                quadCard.innerHTML = `
                    <div class="card">
                        <div class="card-header">
                            <div class="d-flex justify-content-between align-items-center">
                                <h5 class="mb-0">${quad.name}</h5>
                                <div>
                                    <button class="btn btn-sm btn-outline-primary edit-quad" data-id="${quad.id}">Edit</button>
                                    <button class="btn btn-sm btn-outline-danger delete-quad" data-id="${quad.id}">Delete</button>
                                </div>
                            </div>
                        </div>
                        <div class="card-body">
                            ${playersHtml}
                        </div>
                    </div>
                `;

                quadsContainer.appendChild(quadCard);
            });

            // Add event listeners to the buttons
            // (implement these functions later)
        } else {
            document.getElementById('no-quads-message').style.display = 'block';
        }
    });

    // Add event listener for the create quad button
    const createQuadBtn = document.getElementById('create-quad-btn');
    if (createQuadBtn) {
        createQuadBtn.addEventListener('click', function () {
            // Implement quad creation functionality
            console.log('Creating new quad');
        });
    }
}

/**
 * Initialize create tournament page
 * Sets up the form submission handler
 */
function initializeCreateTournamentPage() {
    console.log('Create Tournament page initialized');

    const form = document.getElementById('create-tournament-form');
    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            // Get form values
            const tournament = {
                name: document.getElementById('tournamentName').value,
                date: document.getElementById('tournamentDate').value,
                venueName: document.getElementById('venueName').value,
                street: document.getElementById('street').value,
                city: document.getElementById('city').value,
                state: document.getElementById('state').value,
                postalCode: document.getElementById('postalCode').value
            };

            // Submit the form data to the API
            fetch('/api/main/tournaments', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(tournament)
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Navigate back to the dashboard
                        navigateTo('dashboard');
                        // Add a success message (implement this later)
                    } else {
                        // Show error message
                        console.error('Error creating tournament:', data.error);
                        // Display error to user (implement this later)
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    // Display error to user (implement this later)
                });
        });
    }
}