import { useEffect, useState } from "react";
import { NavLink } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";

interface SoundPack {
    id: string;
    soundPackName: string;
    genre: string;
    packCategory: string;
    createdAt: string;
}

function Dashboard() {
    const { token } = useAuth();
    const [soundPacks, setSoundPacks] = useState<SoundPack[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function fetchSoundPacks() {
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

        fetchSoundPacks();
    }, [token]);

    return (
        <div>
            <h1 style={{ fontSize: 22, fontWeight: 600, marginBottom: 20 }}>Dashboard</h1>

            <div className="grid-3">
                <div className="stat-mini">
                    <div className="label">Sound Packs</div>
                    <div className="value">{soundPacks.length}</div>
                </div>
                <div className="stat-mini">
                    <div className="label">Projects</div>
                    <div className="value">—</div>
                </div>
                <div className="stat-mini">
                    <div className="label">Sessions</div>
                    <div className="value">—</div>
                </div>
            </div>

            <div className="section-title">Sound Packs</div>
            {loading && <p style={{ color: "var(--text-muted)" }}>Loading...</p>}
            {error && <p style={{ color: "var(--text-danger)" }}>{error}</p>}
            {!loading && !error && soundPacks.length === 0 && (
                <p style={{ color: "var(--text-muted)" }}>No sound packs yet.</p>
            )}

            <div className="table-wrap">
                {soundPacks.length > 0 && (
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
                                <td>{pack.genre}</td>
                                <td>{pack.packCategory}</td>
                                <td>{new Date(pack.createdAt).toLocaleDateString()}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                )}
            </div>
        </div>
    );
}

export default Dashboard;