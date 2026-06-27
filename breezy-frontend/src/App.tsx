import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import Register from "./pages/Register";
import Projects from "./pages/Projects.tsx";
import SoundPacks from "./pages/SoundPacks.tsx";
import Sessions from "./pages/Sessions.tsx";
import AppLayout from "./components/layout/AppLayout.tsx";
function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Navigate to="/login" />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route element={<AppLayout />}>
                    <Route path="/dashboard" element={<Dashboard />} />
                    <Route path="/projects" element={<Projects />} />
                    <Route path="/sessions" element={<Sessions />} />
                    <Route path="/soundpacks" element={<SoundPacks />} />
                </Route>


            </Routes>
        </BrowserRouter>
    );
}

export default App;