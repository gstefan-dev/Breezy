import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

function Sidebar() {
    const navigate = useNavigate();
    const { logout } = useAuth();

    function handleLogout() {
        logout();
        navigate("/login");
    }

    return (
        <nav>
            <ul>
                <li>
                    <NavLink to="/dashboard">Dashboard</NavLink>
                </li>
                <li>
                    <NavLink to="/projects">Projects</NavLink>
                </li>
                <li>
                    <NavLink to="/sessions">Sessions</NavLink>
                </li>
                <li>
                    <NavLink to="/soundpacks">SoundPacks</NavLink>
                </li>
            </ul>
            <button onClick={handleLogout}>Logout</button>
        </nav>
    );
}

export default Sidebar;