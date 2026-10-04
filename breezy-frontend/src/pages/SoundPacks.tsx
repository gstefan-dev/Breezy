import { useState } from "react";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";

function SoundPacks() {
    const { token } = useAuth();
    const [packName, setPackName] = useState("");
    const [description, setDescription] = useState("");
    const [genre, setGenre] = useState("");
    const [mood, setMood] = useState("");
    const [packCategory, setPackCategory] = useState("");
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [loading, setLoading] = useState(false);

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        setError("");
        setSuccess("");
        setLoading(true);

        try {
            const response = await fetch(`${API_BASE_URL}/api/soundpacks`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({
                    PackName: packName,
                    Description: description,
                    Genre: genre,
                    Mood: mood,
                    PackCategory: packCategory,
                }),
            });

            if (!response.ok) {
                throw new Error("Failed to create sound pack");
            }

            const pack = await response.json();
            setSuccess(`Sound pack "${pack.packName}" created successfully!`);
            setPackName("");
            setDescription("");
            setGenre("");
            setMood("");
            setPackCategory("");
        } catch (err) {
            console.error(err);
            setError("Could not create sound pack.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div>
            <h1 style={{ fontSize: 22, fontWeight: 600, marginBottom: 20 }}>Sound Packs</h1>

            <div className="section-title" style={{ marginTop: 0 }}>Create New Sound Pack</div>
            <div className="card" style={{ maxWidth: 420 }}>
                <form onSubmit={handleSubmit}>
                    <div>
                        <label htmlFor="packName">Pack Name</label>
                        <input id="packName" type="text" value={packName} onChange={(e) => setPackName(e.target.value)} required />
                    </div>
                    <div>
                        <label htmlFor="description">Description</label>
                        <input id="description" type="text" value={description} onChange={(e) => setDescription(e.target.value)} />
                    </div>
                    <div>
                        <label htmlFor="genre">Genre</label>
                        <input id="genre" type="text" value={genre} onChange={(e) => setGenre(e.target.value)} />
                    </div>
                    <div>
                        <label htmlFor="mood">Mood</label>
                        <input id="mood" type="text" value={mood} onChange={(e) => setMood(e.target.value)} />
                    </div>
                    <div>
                        <label htmlFor="packCategory">Category</label>
                        <input id="packCategory" type="text" value={packCategory} onChange={(e) => setPackCategory(e.target.value)} />
                    </div>
                    {error && <p style={{ color: "var(--text-danger)" }}>{error}</p>}
                    {success && <p style={{ color: "var(--text-success)" }}>{success}</p>}
                    <button type="submit" disabled={loading}>
                        {loading ? "Creating..." : "Create Sound Pack"}
                    </button>
                </form>
            </div>
        </div>
    );
}

export default SoundPacks;