import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { decodeToken } from "../../utils/jwt";

function Sidebar() {
    const navigate = useNavigate();
    const { logout, token } = useAuth();

    const email = token ? decodeToken(token)?.email ?? "" : "";
    const initials = email ? email[0].toUpperCase() : "?";

    function handleLogout() {
        logout();
        navigate("/login");
    }

    return (
        <div className="sidebar">
            <div className="logo">Bree<span>zy</span></div>

            <NavLink to="/dashboard" className="nav-item">
                <i className="ti ti-layout-dashboard" /> Dashboard
            </NavLink>
            <NavLink to="/projects" className="nav-item">
                <i className="ti ti-music" /> Projects
            </NavLink>
            <NavLink to="/soundpacks" className="nav-item">
                <i className="ti ti-package" /> Sound Packs
            </NavLink>
            <NavLink to="/sessions" className="nav-item">
                <i className="ti ti-users" /> Sessions
            </NavLink>

            <div className="user-section">
                <div className="avatar">{initials}</div>
                <div>
                    <div className="user-name">{email}</div>
                    <div className="user-plan" onClick={handleLogout} style={{ cursor: "pointer" }}>
                        Logout
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Sidebar;