import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";

interface Session {
    id: string;
    code: string;
    name: string;
    isActive: boolean;
    hostId: string;
    createdAt: string;
    visibility: number;
}

interface Participant {
    id: string;
    sessionId: string;
    userId: string;
    userName: string;
    joinedAt: string;
}

interface Submission {
    id: string;
    sessionId: string;
    submitterId: string;
    fileName: string;
    fileUrl: string;
    createdAt: string;
}

async function getAudioDuration(file: File): Promise<number> {
    const audioContext = new AudioContext();
    const arrayBuffer = await file.arrayBuffer();
    const audioBuffer = await audioContext.decodeAudioData(arrayBuffer);
    return Math.round(audioBuffer.duration);
}

function SessionDetail() {
    const { code } = useParams();
    const { token } = useAuth();
    const [session, setSession] = useState<Session | null>(null);
    const [participants, setParticipants] = useState<Participant[]>([]);
    const [submissions, setSubmissions] = useState<Submission[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [fileName, setFileName] = useState("");
    const [genre, setGenre] = useState("");
    const [key, setKey] = useState("");
    const [bpm, setBpm] = useState("");
    const [durationSeconds, setDurationSeconds] = useState(0);
    const [file, setFile] = useState<File | null>(null);
    const [submitError, setSubmitError] = useState("");
    const [submitSuccess, setSubmitSuccess] = useState("");
    const [submitting, setSubmitting] = useState(false);

    async function loadSession() {
        try {
            const [sessionRes, participantsRes] = await Promise.all([
                fetch(`${API_BASE_URL}/api/sessions/${code}`, {
                    headers: { Authorization: `Bearer ${token}` },
                }),
                fetch(`${API_BASE_URL}/api/sessions/${code}/participants`, {
                    headers: { Authorization: `Bearer ${token}` },
                }),
            ]);

            if (!sessionRes.ok || !participantsRes.ok) {
                throw new Error("Failed to load session");
            }

            setSession(await sessionRes.json());
            setParticipants(await participantsRes.json());

            const submissionsRes = await fetch(`${API_BASE_URL}/api/sessions/${code}/submissions`, {
                headers: { Authorization: `Bearer ${token}` },
            });

            setSubmissions(submissionsRes.ok ? await submissionsRes.json() : []);
        } catch (err) {
            console.error(err);
            setError("Could not load session.");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        loadSession();
    }, [code, token]);

    async function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
        const selectedFile = e.target.files?.[0] ?? null;
        setFile(selectedFile);

        if (selectedFile) {
            const duration = await getAudioDuration(selectedFile);
            setDurationSeconds(duration);
        }
    }

    async function handleSubmitSample(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        if (!file) {
            setSubmitError("Please select a file.");
            return;
        }

        setSubmitError("");
        setSubmitSuccess("");
        setSubmitting(true);

        try {
            const formData = new FormData();
            formData.append("fileName", fileName);
            formData.append("durationSeconds", durationSeconds.toString());
            formData.append("key", key);
            formData.append("genre", genre);
            formData.append("bpm", bpm);
            formData.append("file", file);

            const response = await fetch(`${API_BASE_URL}/api/sessions/${code}/samples`, {
                method: "POST",
                headers: { Authorization: `Bearer ${token}` },
                body: formData,
            });

            if (!response.ok) {
                throw new Error("Failed to submit sample");
            }

            setSubmitSuccess("Sample submitted successfully!");
            setFileName("");
            setGenre("");
            setKey("");
            setBpm("");
            setDurationSeconds(0);
            setFile(null);

            loadSession();
        } catch (err) {
            console.error(err);
            setSubmitError("Could not submit sample.");
        } finally {
            setSubmitting(false);
        }
    }

    if (loading) return <p style={{ color: "var(--text-muted)" }}>Loading...</p>;
    if (error) return <p style={{ color: "var(--text-danger)" }}>{error}</p>;
    if (!session) return <p style={{ color: "var(--text-muted)" }}>Session not found.</p>;

    return (
        <div>
            <div style={{ display: "flex", alignItems: "center", gap: 10, marginBottom: 16 }}>
                <h1 style={{ fontSize: 22, fontWeight: 600 }}>{session.name}</h1>
                <span className={session.isActive ? "badge badge-green" : "badge badge-red"}>
          {session.isActive ? "Active" : "Closed"}
        </span>
                <span className="badge badge-blue">
          {session.visibility === 1 ? "Collaborative" : "Curated"}
        </span>
            </div>

            <div className="code-box">
                <div className="session-code">{session.code}</div>
                <button type="button" onClick={() => navigator.clipboard.writeText(session.code)}>
                    Copy
                </button>
            </div>

            <div className="grid-3">
                <div className="stat-mini">
                    <div className="label">Participants</div>
                    <div className="value">{participants.length}</div>
                </div>
                <div className="stat-mini">
                    <div className="label">Submissions</div>
                    <div className="value">{submissions.length}</div>
                </div>
                <div className="stat-mini">
                    <div className="label">Created</div>
                    <div className="value" style={{ fontSize: 13 }}>
                        {new Date(session.createdAt).toLocaleDateString()}
                    </div>
                </div>
            </div>

            <div className="section-title">Participants</div>
            <div className="card">
                {participants.length === 0 && (
                    <p style={{ color: "var(--text-muted)", fontSize: 13 }}>No participants yet.</p>
                )}
                {participants.map((p) => (
                    <div key={p.id} className="participant">
                        <div className="avatar">{p.userName[0]?.toUpperCase()}</div>
                        <div style={{ fontSize: 13 }}>{p.userName}</div>
                        <span style={{ marginLeft: "auto", fontSize: 11, color: "var(--text-muted)" }}>
              joined {new Date(p.joinedAt).toLocaleDateString()}
            </span>
                    </div>
                ))}
            </div>

            <div className="section-title">Submissions</div>
            <div className="card">
                {submissions.length === 0 && (
                    <p style={{ color: "var(--text-muted)", fontSize: 13 }}>No submissions yet.</p>
                )}
                {submissions.map((s) => (
                    <div key={s.id} className="audio-file">
                        <div className="play-btn">
                            <i className="ti ti-player-play" style={{ fontSize: 12 }} />
                        </div>
                        <div className="audio-info">
                            <div className="audio-name">{s.fileName}</div>
                            <div className="audio-meta">{new Date(s.createdAt).toLocaleDateString()}</div>
                        </div>
                    </div>
                ))}
            </div>

            <div className="section-title">Submit Sample</div>
            <form onSubmit={handleSubmitSample}>
                <div>
                    <label htmlFor="fileName">File Name</label>
                    <input id="fileName" type="text" value={fileName} onChange={(e) => setFileName(e.target.value)} required />
                </div>
                <div>
                    <label htmlFor="genre">Genre</label>
                    <input id="genre" type="text" value={genre} onChange={(e) => setGenre(e.target.value)} required />
                </div>
                <div>
                    <label htmlFor="key">Key</label>
                    <input id="key" type="text" value={key} onChange={(e) => setKey(e.target.value)} required />
                </div>
                <div>
                    <label htmlFor="bpm">BPM</label>
                    <input id="bpm" type="number" value={bpm} onChange={(e) => setBpm(e.target.value)} required />
                </div>
                <div>
                    <label htmlFor="file">Audio File</label>
                    <input id="file" type="file" accept="audio/*" onChange={handleFileChange} required />
                    {durationSeconds > 0 && (
                        <p style={{ fontSize: 12, color: "var(--text-muted)", marginTop: 4 }}>
                            Duration detected: {durationSeconds}s
                        </p>
                    )}
                </div>
                {submitError && <p style={{ color: "var(--text-danger)" }}>{submitError}</p>}
                {submitSuccess && <p style={{ color: "var(--text-success)" }}>{submitSuccess}</p>}
                <button type="submit" disabled={submitting}>
                    {submitting ? "Submitting..." : "Submit Sample"}
                </button>
            </form>
        </div>
    );
}

export default SessionDetail;