import { useEffect, useState } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";
import PageHeader from "../components/layout/PageHeader";

interface SoundPack {
    id: string;
    soundPackName: string;
    genre: string;
    packCategory: string;
    createdAt: string;
}

function SoundPacks() {
    const { token } = useAuth();
    const navigate = useNavigate();
    const [soundPacks, setSoundPacks] = useState<SoundPack[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [showForm, setShowForm] = useState(false);
    const [packName, setPackName] = useState("");
    const [description, setDescription] = useState("");
    const [genre, setGenre] = useState("");
    const [mood, setMood] = useState("");
    const [packCategory, setPackCategory] = useState("");
    const [createError, setCreateError] = useState("");
    const [creating, setCreating] = useState(false);

    useEffect(() => {
        async function loadSoundPacks() {
            try {
                const response = await fetch(`${API_BASE_URL}/api/soundpacks/user`, {
                    headers: { Authorization: `Bearer ${token}` },
                });

                if (!response.ok) {
                    throw new Error("Failed to load sound packs");
                }

                setSoundPacks(await response.json());
            } catch (err) {
                console.error(err);
                setError("Could not load your sound packs.");
            } finally {
                setLoading(false);
            }
        }

        loadSoundPacks();
    }, [token]);

    async function handleCreate(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        setCreateError("");
        setCreating(true);

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
            navigate(`/soundpacks/${pack.id}`);
        } catch (err) {
            console.error(err);
            setCreateError("Could not create sound pack.");
        } finally {
            setCreating(false);
        }
    }

    return (
        <>
            <PageHeader
                title="Sound Packs"
                actions={
                    <button type="button" className="btn btn-primary" onClick={() => setShowForm(!showForm)}>
                        <i className="ti ti-plus" /> New sound pack
                    </button>
                }
            />

            <div className="content">
                {showForm && (
                    <div className="card" style={{ maxWidth: 520, marginBottom: 16 }}>
                        <form onSubmit={handleCreate}>
                            <div>
                                <label htmlFor="packName">Pack Name</label>
                                <input id="packName" value={packName} onChange={(e) => setPackName(e.target.value)} required />
                            </div>
                            <div>
                                <label htmlFor="description">Description</label>
                                <input id="description" value={description} onChange={(e) => setDescription(e.target.value)} />
                            </div>
                            <div className="grid-3" style={{ marginBottom: 0 }}>
                                <div>
                                    <label htmlFor="genre">Genre</label>
                                    <input id="genre" value={genre} onChange={(e) => setGenre(e.target.value)} />
                                </div>
                                <div>
                                    <label htmlFor="mood">Mood</label>
                                    <input id="mood" value={mood} onChange={(e) => setMood(e.target.value)} />
                                </div>
                                <div>
                                    <label htmlFor="packCategory">Category</label>
                                    <input id="packCategory" value={packCategory} onChange={(e) => setPackCategory(e.target.value)} />
                                </div>
                            </div>
                            {createError && <p style={{ color: "var(--text-danger)", fontSize: 13 }}>{createError}</p>}
                            <button type="submit" disabled={creating} style={{ marginTop: 12 }}>
                                {creating ? "Creating..." : "Create sound pack"}
                            </button>
                        </form>
                    </div>
                )}

                {loading && <p style={{ color: "var(--text-muted)" }}>Loading...</p>}
                {error && <p style={{ color: "var(--text-danger)" }}>{error}</p>}
                {!loading && !error && soundPacks.length === 0 && (
                    <p style={{ color: "var(--text-muted)" }}>No sound packs yet. Create your first one.</p>
                )}

                {soundPacks.length > 0 && (
                    <div className="table-wrap">
                        <table className="table">
                            <thead>
                            <tr>
                                <th>Name</th>
                                <th>Genre</th>
                                <th>Category</th>
                                <th>Created</th>
                            </tr>
                            </thead>
                            <tbody>
                            {soundPacks.map((pack) => (
                                <tr key={pack.id}>
                                    <td>
                                        <NavLink to={`/soundpacks/${pack.id}`} style={{ fontWeight: 500 }}>
                                            {pack.soundPackName}
                                        </NavLink>
                                    </td>
                                    <td>{pack.genre || "—"}</td>
                                    <td>{pack.packCategory}</td>
                                    <td>{new Date(pack.createdAt).toLocaleDateString()}</td>
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

export default SoundPacks;