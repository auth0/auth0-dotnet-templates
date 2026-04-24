import { useAuth } from './AuthContext';

const LoginButton = () => {
  const { login } = useAuth();
  return (
    <button 
      onClick={() => login()} 
      className="button login"
    >
      Log In
    </button>
  );
};

export default LoginButton;