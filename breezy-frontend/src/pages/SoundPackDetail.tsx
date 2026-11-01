import { useEffect, useRef, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";
import PageHeader from "../components/layout/PageHeader";
import { getAudioDuration } from "../utils/files";

interface SoundPackFile {
    id: string;
    fileName: string;
    category: string;
    durationInSeconds: number;
    bpm: number | null;
    key: string | null;
    fileSize: number;
}

interface SoundPackDetail {
    id: string;
    soundPackName: string;
    ownerName: string;
    description: string;
    genre: string;
    mood: string;
    packCategory: string;
    createdAt: string;
    files: SoundPackFile[];
}

const SOUND_CATEGORIES = [
    "Kick", "Snare", "Clap", "HiHat", "Bass", "FX", "Melody", "Synth", "Percussion",
    "Vocal", "Beats", "Loop", "OneShot", "Pad", "String", "Horn", "Instrument", "Other",
];

function formatDuration(seconds: number) {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, "0")}`;
}

function formatSize(bytes: number) {
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

function SoundPackDetail() {
    const { id } = useParams();
    const { token } = useAuth();
    const [soundPack, setSoundPack] = useState<SoundPackDetail | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const fileInputRef = useRef<HTMLInputElement>(null);
    const [showUpload, setShowUpload] = useState(false);
    const [dragging, setDragging] = useState(false);
    const [file, setFile] = useState<File | null>(null);
    const [fileName, setFileName] = useState("");
    const [soundCategory, setSoundCategory] = useState("Kick");
    const [bpm, setBpm] = useState("");
    const [key, setKey] = useState("");
    const [durationSeconds, setDurationSeconds] = useState(0);
    const [uploadError, setUploadError] = useState("");
    const [uploading, setUploading] = useState(false);

    async function loadSoundPack() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/soundpacks/${id}`, {
                headers: { Authorization: `Bearer ${token}` },
            });

            if (!response.ok) {
                throw new Error("Failed to load sound pack");
            }

            setSoundPack(await response.json());
        } catch (err) {
            console.error(err);
            setError("Could not load sound pack.");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        loadSoundPack();
    }, [id, token]);

    async function selectFile(selected: File) {
        setFile(selected);
        setFileName(selected.name.replace(/\.[^.]+$/, ""));
        setDurationSeconds(await getAudioDuration(selected));
    }

    function handleDrop(e: React.DragEvent<HTMLDivElement>) {
        e.preventDefault();
        setDragging(false);
        const dropped = e.dataTransfer.files[0];
        if (dropped) selectFile(dropped);
    }

    function resetUpload() {
        setFile(null);
        setFileName("");
        setSoundCategory("Kick");
        setBpm("");
        setKey("");
        setDurationSeconds(0);
        if (fileInputRef.current) fileInputRef.current.value = "";
    }

    async function handleUpload(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        if (!file) return;

        setUploadError("");
        setUploading(true);

        try {
            const formData = new FormData();
            formData.append("fileName", fileName);
            formData.append("soundCategory", soundCategory);
            formData.append("durationSeconds", durationSeconds.toString());
            if (bpm) formData.append("bpm", bpm);
            if (key) formData.append("key", key);
            formData.append("file", file);

            const response = await fetch(`${API_BASE_URL}/api/soundpacks/${id}/files`, {
                method: "POST",
                headers: { Authorization: `Bearer ${token}` },
                body: formData,
            });

            if (!response.ok) {
                throw new Error("Failed to upload file");
            }

            resetUpload();
            setShowUpload(false);
            loadSoundPack();
        } catch (err) {
            console.error(err);
            setUploadError("Could not upload file.");
        } finally {
            setUploading(false);
        }
    }

    async function handleDelete(fileId: string) {
        const response = await fetch(`${API_BASE_URL}/api/soundpacks/${id}/files/${fileId}`, {
            method: "DELETE",
            headers: { Authorization: `Bearer ${token}` },
        });

        if (response.ok) loadSoundPack();
    }

    if (loading) return <div className="content"><p style={{ color: "var(--text-muted)" }}>Loading...</p></div>;
    if (error) return <div className="content"><p style={{ color: "var(--text-danger)" }}>{error}</p></div>;
    if (!soundPack) return <div className="content"><p style={{ color: "var(--text-muted)" }}>Sound pack not found.</p></div>;

    const totalSize = soundPack.files.reduce((sum, f) => sum + f.fileSize, 0);
    const totalDuration = soundPack.files.reduce((sum, f) => sum + f.durationInSeconds, 0);
    const canDelete = soundPack.files.length > 1;

    return (
        <>
            <PageHeader
                title={soundPack.soundPackName}
                backTo="/soundpacks"
                backLabel="Sound Packs"
                badge={<span className="badge badge-blue">{soundPack.packCategory}</span>}
                actions={
                    <button type="button" className="btn btn-primary" onClick={() => setShowUpload(!showUpload)}>
                        <i className="ti ti-plus" /> Add file
                    </button>
                }
            />

            <div className="content">
                {showUpload && (
                    <div style={{ marginBottom: 16 }}>
                        {!file ? (
                            <div
                                className={`dropzone ${dragging ? "drag" : ""}`}
                                onClick={() => fileInputRef.current?.click()}
                                onDragOver={(e) => { e.preventDefault(); setDragging(true); }}
                                onDragLeave={() => setDragging(false)}
                                onDrop={handleDrop}
                            >
                                <i className="ti ti-upload" style={{ fontSize: 24, color: "var(--text-muted)" }} />
                                <div style={{ fontSize: 13, color: "var(--text-secondary)", marginTop: 8 }}>
                                    Drag an audio file here or click to choose
                                </div>
                            </div>
                        ) : (
                            <div className="card">
                                <form onSubmit={handleUpload}>
                                    <div className="audio-file">
                                        <div className="play-btn"><i className="ti ti-music" style={{ fontSize: 13 }} /></div>
                                        <div className="audio-info">
                                            <div className="audio-name">{file.name}</div>
                                            <div className="audio-meta">{formatDuration(durationSeconds)} · {formatSize(file.size)}</div>
                                        </div>
                                        <button type="button" className="btn" onClick={resetUpload}><i className="ti ti-x" /></button>
                                    </div>

                                    <div className="grid-4">
                                        <div>
                                            <label htmlFor="fileName">Name</label>
                                            <input id="fileName" value={fileName} onChange={(e) => setFileName(e.target.value)} required />
                                        </div>
                                        <div>
                                            <label htmlFor="soundCategory">Category</label>
                                            <select id="soundCategory" value={soundCategory} onChange={(e) => setSoundCategory(e.target.value)}>
                                                {SOUND_CATEGORIES.map((c) => (
                                                    <option key={c} value={c}>{c}</option>
                                                ))}
                                            </select>
                                        </div>
                                        <div>
                                            <label htmlFor="bpm">BPM</label>
                                            <input id="bpm" type="number" value={bpm} onChange={(e) => setBpm(e.target.value)} placeholder="Optional" />
                                        </div>
                                        <div>
                                            <label htmlFor="key">Key</label>
                                            <input id="key" value={key} onChange={(e) => setKey(e.target.value)} placeholder="Optional" />
                                        </div>
                                    </div>

                                    {uploadError && <p style={{ color: "var(--text-danger)", fontSize: 13 }}>{uploadError}</p>}
                                    <button type="submit" disabled={uploading} style={{ marginTop: 12 }}>
                                        {uploading ? "Uploading..." : "Upload file"}
                                    </button>
                                </form>
                            </div>
                        )}
                    </div>
                )}

                <div className="grid-3">
                    <div className="stat-mini">
                        <div className="label">Files</div>
                        <div className="value">{soundPack.files.length}</div>
                    </div>
                    <div className="stat-mini">
                        <div className="label">Total size</div>
                        <div className="value">{formatSize(totalSize)}</div>
                    </div>
                    <div className="stat-mini">
                        <div className="label">Total duration</div>
                        <div className="value">{formatDuration(totalDuration)}</div>
                    </div>
                </div>

                <div className="section-title">Audio files</div>
                {soundPack.files.length === 0 && (
                    <p style={{ color: "var(--text-muted)", fontSize: 13 }}>No files yet. Add your first one.</p>
                )}
                {soundPack.files.map((f) => (
                    <div key={f.id} className="audio-file">
                        <div className="play-btn"><i className="ti ti-music" style={{ fontSize: 13 }} /></div>
                        <div className="audio-info">
                            <div className="audio-name">{f.fileName}</div>
                            <div className="audio-meta">
                                {[
                                    f.category,
                                    formatDuration(f.durationInSeconds),
                                    f.bpm && `${f.bpm} BPM`,
                                    f.key,
                                    formatSize(f.fileSize),
                                ].filter(Boolean).join(" · ")}
                            </div>
                        </div>
                        <button
                            type="button"
                            className="btn"
                            onClick={() => handleDelete(f.id)}
                            disabled={!canDelete}
                            title={canDelete ? "Delete file" : "A pack must contain at least one file"}
                        >
                            <i className="ti ti-trash" />
                        </button>
                    </div>
                ))}

                <input
                    ref={fileInputRef}
                    type="file"
                    accept="audio/*"
                    hidden
                    onChange={(e) => {
                        const selected = e.target.files?.[0];
                        if (selected) selectFile(selected);
                    }}
                />
            </div>
        </>
    );
}

export default SoundPackDetail;