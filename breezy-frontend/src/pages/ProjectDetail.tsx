import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";

interface Project {
    id: string;
    userId: string;
    projectName: string;
    createdAt: string;
}

interface ProjectVersion {
    id: string;
    projectId: string;
    versionNumber: number;
    description: string | null;
    createdAt: string;
}

function ProjectDetail() {
    const { id } = useParams();
    const { token } = useAuth();
    const [project, setProject] = useState<Project | null>(null);
    const [versions, setVersions] = useState<ProjectVersion[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function fetchProject() {
            try {
                const [projectRes, versionsRes] = await Promise.all([
                    fetch(`${API_BASE_URL}/api/projects/${id}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                    fetch(`${API_BASE_URL}/api/projects/${id}/versions`, {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                ]);

                if (!projectRes.ok || !versionsRes.ok) {
                    throw new Error("Failed to load project");
                }

                setProject(await projectRes.json());
                setVersions(await versionsRes.json());
            } catch (err) {
                console.error(err);
                setError("Could not load project.");
            } finally {
                setLoading(false);
            }
        }

        fetchProject();
    }, [id, token]);

    if (loading) return <p style={{ color: "var(--text-muted)" }}>Loading...</p>;
    if (error) return <p style={{ color: "var(--text-danger)" }}>{error}</p>;
    if (!project) return <p style={{ color: "var(--text-muted)" }}>Project not found.</p>;

    const latestVersion = versions.length > 0 ? Math.max(...versions.map(v => v.versionNumber)) : 0;

    return (
        <div>
            <div style={{ display: "flex", alignItems: "center", gap: 10, marginBottom: 16 }}>
                <h1 style={{ fontSize: 22, fontWeight: 600 }}>{project.projectName}</h1>
                {latestVersion > 0 && <span className="badge badge-blue">v{latestVersion}</span>}
            </div>

            <div className="grid-3">
                <div className="stat-mini">
                    <div className="label">Versions</div>
                    <div className="value">{versions.length}</div>
                </div>
                <div className="stat-mini">
                    <div className="label">Created</div>
                    <div className="value" style={{ fontSize: 13 }}>
                        {new Date(project.createdAt).toLocaleDateString()}
                    </div>
                </div>
            </div>

            <div className="section-title">Version History</div>
            <div className="table-wrap">
                {versions.length === 0 ? (
                    <p style={{ color: "var(--text-muted)", fontSize: 13, padding: 16 }}>No versions yet.</p>
                ) : (
                    <table className="table">
                        <thead>
                        <tr>
                            <th>Version</th>
                            <th>Description</th>
                            <th>Date</th>
                        </tr>
                        </thead>
                        <tbody>
                        {versions
                            .slice()
                            .sort((a, b) => b.versionNumber - a.versionNumber)
                            .map((version) => (
                                <tr key={version.id}>
                                    <td>
                      <span className={version.versionNumber === latestVersion ? "badge badge-blue" : "badge badge-amber"}>
                        v{version.versionNumber}
                      </span>
                                        {version.versionNumber === latestVersion && (
                                            <span style={{ fontSize: 11, color: "var(--text-muted)", marginLeft: 6 }}>current</span>
                                        )}
                                    </td>
                                    <td>{version.description || "—"}</td>
                                    <td>{new Date(version.createdAt).toLocaleDateString()}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>
        </div>
    );
}

export default ProjectDetail;