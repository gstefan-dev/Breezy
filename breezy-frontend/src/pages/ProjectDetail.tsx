import { useEffect, useRef, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";
import PageHeader from "../components/layout/PageHeader";
import { computeHash } from "../utils/files";

interface Project {
    id: string;
    projectName: string;
    createdAt: string;
}

interface ProjectVersion {
    id: string;
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

    const fileInputRef = useRef<HTMLInputElement>(null);
    const [showUpload, setShowUpload] = useState(false);
    const [dragging, setDragging] = useState(false);
    const [uploading, setUploading] = useState(false);
    const [uploadError, setUploadError] = useState("");

    const [changelog, setChangelog] = useState("");
    const [changelogError, setChangelogError] = useState("");

    async function loadProject() {
        try {
            const headers = { Authorization: `Bearer ${token}` };
            const [projectRes, versionsRes] = await Promise.all([
                fetch(`${API_BASE_URL}/api/projects/${id}`, { headers }),
                fetch(`${API_BASE_URL}/api/projects/${id}/versions`, { headers }),
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

    useEffect(() => {
        loadProject();
    }, [id, token]);

    async function uploadVersion(file: File) {
        setUploadError("");
        setUploading(true);

        try {
            const formData = new FormData();
            formData.append("hash", await computeHash(file));
            formData.append("file", file);

            const response = await fetch(`${API_BASE_URL}/api/projects/${id}/versions`, {
                method: "POST",
                headers: { Authorization: `Bearer ${token}` },
                body: formData,
            });

            if (!response.ok) {
                throw new Error("Failed to upload version");
            }

            setShowUpload(false);
            loadProject();
        } catch (err) {
            console.error(err);
            setUploadError("Could not upload version.");
        } finally {
            setUploading(false);
            if (fileInputRef.current) fileInputRef.current.value = "";
        }
    }

    function handleDrop(e: React.DragEvent<HTMLDivElement>) {
        e.preventDefault();
        setDragging(false);
        const dropped = e.dataTransfer.files[0];
        if (dropped) uploadVersion(dropped);
    }

    const sorted = [...versions].sort((a, b) => b.versionNumber - a.versionNumber);
    const latest = sorted[0];

    async function handleSaveChangelog(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        if (!latest) return;
        setChangelogError("");

        const response = await fetch(
            `${API_BASE_URL}/api/projects/${id}/versions/${latest.versionNumber}/changelog`,
            {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ Changelog: changelog }),
            }
        );

        if (response.ok) {
            setChangelog("");
            loadProject();
        } else {
            setChangelogError("Could not save changelog.");
        }
    }

    if (loading) return <div className="content"><p style={{ color: "var(--text-muted)" }}>Loading...</p></div>;
    if (error) return <div className="content"><p style={{ color: "var(--text-danger)" }}>{error}</p></div>;
    if (!project) return <div className="content"><p style={{ color: "var(--text-muted)" }}>Project not found.</p></div>;

    return (
        <>
            <PageHeader
                title={project.projectName}
                backTo="/projects"
                backLabel="Projects"
                badge={latest && <span className="badge badge-blue">v{latest.versionNumber}</span>}
                actions={
                    <button type="button" className="btn btn-primary" onClick={() => setShowUpload(!showUpload)}>
                        <i className="ti ti-upload" /> New version
                    </button>
                }
            />

            <div className="content">
                {showUpload && (
                    <div style={{ marginBottom: 16 }}>
                        <div
                            className={`dropzone ${dragging ? "drag" : ""}`}
                            onClick={() => fileInputRef.current?.click()}
                            onDragOver={(e) => { e.preventDefault(); setDragging(true); }}
                            onDragLeave={() => setDragging(false)}
                            onDrop={handleDrop}
                        >
                            <i className="ti ti-upload" style={{ fontSize: 24, color: "var(--text-muted)" }} />
                            <div style={{ fontSize: 13, color: "var(--text-secondary)", marginTop: 8 }}>
                                {uploading ? "Uploading..." : "Drag the updated project file here or click to choose"}
                            </div>
                        </div>
                        {uploadError && <p style={{ color: "var(--text-danger)", fontSize: 13, marginTop: 6 }}>{uploadError}</p>}
                    </div>
                )}

                <div className="grid-3">
                    <div className="stat-mini">
                        <div className="label">Versions</div>
                        <div className="value">{versions.length}</div>
                    </div>
                    <div className="stat-mini">
                        <div className="label">Created</div>
                        <div className="value" style={{ fontSize: 13 }}>{new Date(project.createdAt).toLocaleDateString()}</div>
                    </div>
                    <div className="stat-mini">
                        <div className="label">Last update</div>
                        <div className="value" style={{ fontSize: 13 }}>
                            {latest ? new Date(latest.createdAt).toLocaleDateString() : "—"}
                        </div>
                    </div>
                </div>

                <div className="section-title">Version history</div>
                <div className="table-wrap">
                    {sorted.length === 0 ? (
                        <p style={{ color: "var(--text-muted)", fontSize: 13, padding: 16 }}>No versions yet.</p>
                    ) : (
                        <table className="table">
                            <thead>
                            <tr>
                                <th>Version</th>
                                <th>Changelog</th>
                                <th>Date</th>
                            </tr>
                            </thead>
                            <tbody>
                            {sorted.map((v) => (
                                <tr key={v.id}>
                                    <td>
                                        <span className={v === latest ? "badge badge-blue" : "badge badge-amber"}>v{v.versionNumber}</span>
                                        {v === latest && <span style={{ fontSize: 11, color: "var(--text-muted)", marginLeft: 6 }}>current</span>}
                                    </td>
                                    <td>{v.description || "—"}</td>
                                    <td>{new Date(v.createdAt).toLocaleDateString()}</td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    )}
                </div>

                {latest && (
                    <>
                        <div className="section-title">Add changelog to v{latest.versionNumber}</div>
                        <form onSubmit={handleSaveChangelog}>
                            <div>
                <textarea
                    rows={3}
                    value={changelog}
                    onChange={(e) => setChangelog(e.target.value)}
                    placeholder="Describe the changes in this version..."
                    required
                />
                            </div>
                            {changelogError && <p style={{ color: "var(--text-danger)", fontSize: 13 }}>{changelogError}</p>}
                            <button type="submit" className="btn">Save changelog</button>
                        </form>
                    </>
                )}

                <input
                    ref={fileInputRef}
                    type="file"
                    hidden
                    onChange={(e) => {
                        const selected = e.target.files?.[0];
                        if (selected) uploadVersion(selected);
                    }}
                />
            </div>
        </>
    );
}

export default ProjectDetail;