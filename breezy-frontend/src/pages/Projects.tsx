import { useEffect, useState } from "react";
import { NavLink } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";
import PageHeader from "../components/layout/PageHeader";
import { computeHash } from "../utils/files";

interface Project {
    id: string;
    projectName: string;
    createdAt: string;
}

function Projects() {
    const { token } = useAuth();
    const [projects, setProjects] = useState<Project[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [showForm, setShowForm] = useState(false);
    const [projectName, setProjectName] = useState("");
    const [file, setFile] = useState<File | null>(null);
    const [createError, setCreateError] = useState("");
    const [creating, setCreating] = useState(false);

    async function loadProjects() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/projects/user`, {
                headers: { Authorization: `Bearer ${token}` },
            });

            if (!response.ok) {
                throw new Error("Failed to load projects");
            }

            setProjects(await response.json());
        } catch (err) {
            console.error(err);
            setError("Could not load your projects.");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        loadProjects();
    }, [token]);

    async function handleCreate(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        if (!file) return;

        setCreateError("");
        setCreating(true);

        try {
            const formData = new FormData();
            formData.append("projectName", projectName);
            formData.append("hash", await computeHash(file));
            formData.append("file", file);

            const response = await fetch(`${API_BASE_URL}/api/projects`, {
                method: "POST",
                headers: { Authorization: `Bearer ${token}` },
                body: formData,
            });

            if (!response.ok) {
                throw new Error("Failed to create project");
            }

            setProjectName("");
            setFile(null);
            setShowForm(false);
            loadProjects();
        } catch (err) {
            console.error(err);
            setCreateError("Could not create project.");
        } finally {
            setCreating(false);
        }
    }

    return (
        <>
            <PageHeader
                title="Projects"
                actions={
                    <button type="button" className="btn btn-primary" onClick={() => setShowForm(!showForm)}>
                        <i className="ti ti-plus" /> New project
                    </button>
                }
            />

            <div className="content">
                {showForm && (
                    <div className="card" style={{ maxWidth: 420, marginBottom: 16 }}>
                        <form onSubmit={handleCreate}>
                            <div>
                                <label htmlFor="projectName">Project Name</label>
                                <input
                                    id="projectName"
                                    value={projectName}
                                    onChange={(e) => setProjectName(e.target.value)}
                                    required
                                />
                            </div>
                            <div>
                                <label htmlFor="file">Project File</label>
                                <input
                                    id="file"
                                    type="file"
                                    onChange={(e) => setFile(e.target.files?.[0] ?? null)}
                                    required
                                />
                            </div>
                            {createError && <p style={{ color: "var(--text-danger)", fontSize: 13 }}>{createError}</p>}
                            <button type="submit" disabled={creating}>
                                {creating ? "Creating..." : "Create project"}
                            </button>
                        </form>
                    </div>
                )}

                {loading && <p style={{ color: "var(--text-muted)" }}>Loading...</p>}
                {error && <p style={{ color: "var(--text-danger)" }}>{error}</p>}
                {!loading && !error && projects.length === 0 && (
                    <p style={{ color: "var(--text-muted)" }}>No projects yet. Create your first one.</p>
                )}

                {projects.length > 0 && (
                    <div className="table-wrap">
                        <table className="table">
                            <thead>
                            <tr>
                                <th>Name</th>
                                <th>Created</th>
                            </tr>
                            </thead>
                            <tbody>
                            {projects.map((project) => (
                                <tr key={project.id}>
                                    <td>
                                        <NavLink to={`/projects/${project.id}`} style={{ fontWeight: 500 }}>
                                            {project.projectName}
                                        </NavLink>
                                    </td>
                                    <td>{new Date(project.createdAt).toLocaleDateString()}</td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </>
    );
}

export default Projects;