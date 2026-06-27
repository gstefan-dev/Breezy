import { NavLink } from "react-router-dom";

function Sidebar() {
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
        </nav>
    )
}

export default Sidebar