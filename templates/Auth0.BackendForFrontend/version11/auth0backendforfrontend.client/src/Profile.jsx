import { useAuth } from './AuthContext';

const Profile = () => {
  const { isAuthenticated, user } = useAuth();

  const getUserName = function (claims) {
    const nameClaim = claims.find(claim => claim.type === 'name');
    return nameClaim ? nameClaim.value : 'User';
  };

  return (
    isAuthenticated && user ? (
      <div>
        <h2>Welcome, {getUserName(user)}!</h2>
      </div>
    ) : null
  );
};

export default Profile;