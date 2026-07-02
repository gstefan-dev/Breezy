import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

interface SoundPackDetail {
    id: string;
    soundPackName: string;
    ownerName: string;
    genre: string;
    packCategory: string;
    createdAt: string;
}

function SoundPackDetail() {
    const { id } = useParams();
    const { token } = useAuth();
    const [soundPack, setSoundPack] = useState<SoundPackDetail | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function fetchSoundPack() {
            try {
                const response = await fetch(`https://localhost:44365/api/soundpacks/${id}`, {
                    headers: { Authorization: `Bearer ${token}` },
                });

                if (!response.ok) {
                    throw new Error("Failed to load sound pack");
                }

                const data = await response.json();
                setSoundPack(data);
            } catch (err) {
                console.error(err);
                setError("Could not load sound pack.");
            } finally {
                setLoading(false);
            }
        }

        fetchSoundPack();
    }, [id, token]);

    if (loading) return <p>Loading...</p>;
    if (error) return <p style={{ color: "red" }}>{error}</p>;
    if (!soundPack) return <p>Sound pack not found.</p>;

    return (
        <div>
            <h1>{soundPack.soundPackName}</h1>
            <p>Owner: {soundPack.ownerName}</p>
            <p>Genre: {soundPack.genre}</p>
            <p>Category: {soundPack.packCategory}</p>
            <p>Created: {new Date(soundPack.createdAt).toLocaleDateString()}</p>
        </div>
    );
}

export default SoundPackDetail;