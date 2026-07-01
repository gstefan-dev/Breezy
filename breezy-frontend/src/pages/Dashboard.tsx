import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";

interface SoundPack {
    id: string;
    soundPackName: string;
    genre: string;
}

function Dashboard() {
    const { token } = useAuth();
    const [soundPacks, setSoundPacks] = useState<SoundPack[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function fetchSoundPacks() {
            try {
                const response = await fetch("https://localhost:44365/api/soundpacks/user", {
                    headers: { Authorization: `Bearer ${token}` },
                });

                if (!response.ok) {
                    throw new Error("Failed to load sound packs");
                }

                const data = await response.json();
                setSoundPacks(data);
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
            <h1>Dashboard</h1>

            <section>
                <h2>Sound Packs</h2>
                {loading && <p>Loading...</p>}
                {error && <p style={{ color: "red" }}>{error}</p>}
                {!loading && !error && soundPacks.length === 0 && (
                    <p>No sound packs yet.</p>
                )}
                <ul>
                    {soundPacks.map((pack) => (
                        <li key={pack.id}>
                            {pack.soundPackName} — {pack.genre}
                        </li>
                    ))}
                </ul>
            </section>

            <section>
                <h2>Projects</h2>
                <p>Listing not available yet.</p>
            </section>

            <section>
                <h2>Sessions</h2>
                <p>Listing not available yet.</p>
            </section>
        </div>
    );
}

export default Dashboard;