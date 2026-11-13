import { useAuth } from './AuthContext';

const LogoutButton = () => {
  const { logout } = useAuth();
  return (
    <button
      onClick={() => logout()}
      className="button logout"
    >
      Log Out
    </button>
  );
};

export default LogoutButton;