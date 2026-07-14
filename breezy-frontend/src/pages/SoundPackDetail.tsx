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

async function getAudioDuration(file: File): Promise<number> {
    const audioContext = new AudioContext();
    const arrayBuffer = await file.arrayBuffer();
    const audioBuffer = await audioContext.decodeAudioData(arrayBuffer);
    return Math.round(audioBuffer.duration);
}

function SoundPackDetail() {
    const { id } = useParams();
    const { token } = useAuth();
    const [soundPack, setSoundPack] = useState<SoundPackDetail | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [fileName, setFileName] = useState("");
    const [soundCategory, setSoundCategory] = useState("0");
    const [durationSeconds, setDurationSeconds] = useState(0);
    const [bpm, setBpm] = useState("");
    const [key, setKey] = useState("");
    const [file, setFile] = useState<File | null>(null);
    const [uploadError, setUploadError] = useState("");
    const [uploadSuccess, setUploadSuccess] = useState("");
    const [uploading, setUploading] = useState(false);

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

    async function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
        const selectedFile = e.target.files?.[0] ?? null;
        setFile(selectedFile);

        if (selectedFile) {
            const duration = await getAudioDuration(selectedFile);
            setDurationSeconds(duration);
        }
    }

    async function handleUpload(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        if (!file) {
            setUploadError("Please select a file.");
            return;
        }

        setUploadError("");
        setUploadSuccess("");
        setUploading(true);

        try {
            const formData = new FormData();
            formData.append("fileName", fileName);
            formData.append("soundCategory", soundCategory);
            formData.append("durationSeconds", durationSeconds.toString());
            formData.append("bpm", bpm);
            formData.append("key", key);
            formData.append("file", file);

            const response = await fetch(`https://localhost:44365/api/soundpacks/${id}/files`, {
                method: "POST",
                headers: { Authorization: `Bearer ${token}` },
                body: formData,
            });

            if (!response.ok) {
                throw new Error("Failed to upload file");
            }

            setUploadSuccess("File uploaded successfully!");
            setFileName("");
            setSoundCategory("0");
            setDurationSeconds(0);
            setBpm("");
            setKey("");
            setFile(null);
        } catch (err) {
            console.error(err);
            setUploadError("Could not upload file.");
        } finally {
            setUploading(false);
        }
    }

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

            <h2>Add File</h2>
            <form onSubmit={handleUpload}>
                <div>
                    <label htmlFor="fileName">File Name</label>
                    <input
                        id="fileName"
                        type="text"
                        value={fileName}
                        onChange={(e) => setFileName(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="soundCategory">Sound Category</label>
                    <select
                        id="soundCategory"
                        value={soundCategory}
                        onChange={(e) => setSoundCategory(e.target.value)}
                    >
                        <option value="0">Drums</option>
                        <option value="1">Bass</option>
                        <option value="2">Melody</option>
                        <option value="3">FX</option>
                        <option value="4">Vocal</option>
                        <option value="5">Other</option>
                    </select>
                </div>
                <div>
                    <label htmlFor="bpm">BPM</label>
                    <input
                        id="bpm"
                        type="number"
                        value={bpm}
                        onChange={(e) => setBpm(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="key">Key</label>
                    <input
                        id="key"
                        type="text"
                        value={key}
                        onChange={(e) => setKey(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="file">Audio File</label>
                    <input
                        id="file"
                        type="file"
                        accept="audio/*"
                        onChange={handleFileChange}
                        required
                    />
                    {durationSeconds > 0 && (
                        <p>Duration detected: {durationSeconds} seconds</p>
                    )}
                </div>
                {uploadError && <p style={{ color: "red" }}>{uploadError}</p>}
                {uploadSuccess && <p style={{ color: "green" }}>{uploadSuccess}</p>}
                <button type="submit" disabled={uploading}>
                    {uploading ? "Uploading..." : "Add File"}
                </button>
            </form>
        </div>
    );
}

export default SoundPackDetail;