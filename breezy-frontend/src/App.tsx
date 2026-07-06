import {BrowserRouter, Routes, Route, Navigate} from "react-router-dom";
import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import Register from "./pages/Register";
import Projects from "./pages/Projects.tsx";
import SoundPacks from "./pages/SoundPacks.tsx";
import Sessions from "./pages/Sessions.tsx";
import AppLayout from "./components/layout/AppLayout.tsx";
import {AuthProvider} from "./context/AuthContext.tsx";
import ProtectedRoute from "./components/ProtectedRoute.tsx";
import SoundPackDetail from "./pages/SoundPackDetail.tsx";
import ProjectDetail from "./pages/ProjectDetail.tsx";
import SessionDetail from "./pages/SessionDetail.tsx";

function App() {
    return (
        <BrowserRouter>
            <AuthProvider>
                <Routes>
                    <Route path="/" element={<Navigate to="/login" />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />

                    <Route element={<ProtectedRoute />}>
                        <Route element={<AppLayout />}>
                            <Route path="/dashboard" element={<Dashboard />} />
                            <Route path="/projects" element={<Projects />} />
                            <Route path="/sessions" element={<Sessions />} />
                            <Route path="/soundpacks" element={<SoundPacks />} />
                            <Route path="/soundpacks/:id" element={<SoundPackDetail />} />
                            <Route path="/projects/:id" element={<ProjectDetail />} />
                            <Route path="/sessions/:code" element={<SessionDetail />} />
                        </Route>
                    </Route>
                </Routes>
            </AuthProvider>
        </BrowserRouter>
    );
}

export default App;