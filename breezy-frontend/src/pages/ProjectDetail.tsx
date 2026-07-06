import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

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
                    fetch(`https://localhost:44365/api/projects/${id}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                    fetch(`https://localhost:44365/api/projects/${id}/versions`, {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                ]);

                if (!projectRes.ok || !versionsRes.ok) {
                    throw new Error("Failed to load project");
                }

                const projectData = await projectRes.json();
                const versionsData = await versionsRes.json();

                setProject(projectData);
                setVersions(versionsData);
            } catch (err) {
                console.error(err);
                setError("Could not load project.");
            } finally {
                setLoading(false);
            }
        }

        fetchProject();
    }, [id, token]);

    if (loading) return <p>Loading...</p>;
    if (error) return <p style={{ color: "red" }}>{error}</p>;
    if (!project) return <p>Project not found.</p>;

    return (
        <div>
            <h1>{project.projectName}</h1>
            <p>Created: {new Date(project.createdAt).toLocaleDateString()}</p>

            <h2>Versions</h2>
            {versions.length === 0 && <p>No versions yet.</p>}
            <ul>
                {versions.map((version) => (
                    <li key={version.id}>
                        <strong>v{version.versionNumber}</strong>
                        {version.description && ` — ${version.description}`}
                        {" "}({new Date(version.createdAt).toLocaleDateString()})
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default ProjectDetail;