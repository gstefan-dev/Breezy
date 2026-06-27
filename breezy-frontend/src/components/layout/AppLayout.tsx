import { Outlet } from "react-router-dom";
import Sidebar from "./Sidebar";

function AppLayout() {
    return (
        <div style={{ display: "flex" }}>
            <Sidebar />
            <main style={{ flex: 1, padding: "1rem" }}>
                <Outlet />
            </main>
        </div>
    );
}

export default AppLayout;