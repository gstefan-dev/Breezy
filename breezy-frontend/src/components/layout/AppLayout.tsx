import { Outlet } from "react-router-dom";
import Sidebar from "./Sidebar";

function AppLayout() {
    return (
        <div className="app">
            <Sidebar />
            <div className="main">
                <div className="topbar">
                    <div className="page-title">Breezy</div>
                </div>
                <div className="content">
                    <Outlet />
                </div>
            </div>
        </div>
    );
}

export default AppLayout;