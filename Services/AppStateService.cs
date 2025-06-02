// Services/AppStateService.cs
public class AppStateService
{
    private string _userName = "";
    private string _userRole = "";
    private bool _isAuthenticated = false;
    private int _tournamentId = 1;

    public string UserName
    {
        get => _userName;
        set
        {
            _userName = value;
            NotifyStateChanged();  // Automatically notifies on change
        }
    }

    public string UserRole
    {
        get => _userRole;

        set
        {
            _userRole = value;
            NotifyStateChanged();  // Automatically notifies on change
        }
    }

    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        set
        {
            _isAuthenticated = value;
            NotifyStateChanged();
        }
    }

    public int TournamentId
    {
        get => _tournamentId;
        set
        {
            _tournamentId = value;
            NotifyStateChanged();  // All subscribed components update!
        }
    }

    public event Action? OnChange;

    public void SetUser(string name, string role)
    {
        UserName = name;
        UserRole = role;
        IsAuthenticated = true;
        TournamentId = 0; // Reset TournamentId on user login
        NotifyStateChanged();
    }

    public void Logout()
    {
        UserName = "";
        UserRole = "";
        IsAuthenticated = false;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}