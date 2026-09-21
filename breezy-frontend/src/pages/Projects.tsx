import { useState } from "react";
import { useAuth } from "../context/AuthContext";

function Projects() {
    const { token } = useAuth();
    const [projectName, setProjectName] = useState("");
    const [file, setFile] = useState<File | null>(null);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [loading, setLoading] = useState(false);

    async function computeHash(file: File): Promise<string> {
        const buffer = await file.arrayBuffer();
        const hashBuffer = await crypto.subtle.digest("SHA-256", buffer);
        const hashArray = Array.from(new Uint8Array(hashBuffer));
        return hashArray.map(b => b.toString(16).padStart(2, "0")).join("");
    }

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        if (!file) {
            setError("Please select a file.");
            return;
        }

        setError("");
        setSuccess("");
        setLoading(true);

        try {
            const hash = await computeHash(file);

            const formData = new FormData();
            formData.append("projectName", projectName);
            formData.append("hash", hash);
            formData.append("file", file);

            const response = await fetch("https://localhost:44365/api/projects", {
                method: "POST",
                headers: { Authorization: `Bearer ${token}` },
                body: formData,
            });

            if (!response.ok) {
                throw new Error("Failed to create project");
            }

            const project = await response.json();
            setSuccess(`Project "${project.projectName}" created successfully!`);
            setProjectName("");
            setFile(null);
        } catch (err) {
            console.error(err);
            setError("Could not create project.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div>
            <h1>Projects</h1>
            <h2>Create New Project</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="projectName">Project Name</label>
                    <input
                        id="projectName"
                        type="text"
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
                {error && <p style={{ color: "red" }}>{error}</p>}
                {success && <p style={{ color: "green" }}>{success}</p>}
                <button type="submit" disabled={loading}>
                    {loading ? "Creating..." : "Create Project"}
                </button>
            </form>
        </div>
    );
}

export default Projects;