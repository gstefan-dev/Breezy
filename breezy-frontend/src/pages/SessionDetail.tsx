import { useEffect, useRef, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";
import PageHeader from "../components/layout/PageHeader";

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
    userId: string;
    userName: string;
    joinedAt: string;
}

interface Submission {
    id: string;
    submitterId: string;
    fileName: string;
    fileUrl: string;
    createdAt: string;
}

async function getAudioDuration(file: File): Promise<number> {
    const audioContext = new AudioContext();
    const audioBuffer = await audioContext.decodeAudioData(await file.arrayBuffer());
    return Math.round(audioBuffer.duration);
}

function SessionDetail() {
    const { code } = useParams();
    const { token, userId } = useAuth();
    const [session, setSession] = useState<Session | null>(null);
    const [participants, setParticipants] = useState<Participant[]>([]);
    const [submissions, setSubmissions] = useState<Submission[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const fileInputRef = useRef<HTMLInputElement>(null);
    const [dragging, setDragging] = useState(false);
    const [file, setFile] = useState<File | null>(null);
    const [fileName, setFileName] = useState("");
    const [genre, setGenre] = useState("");
    const [key, setKey] = useState("");
    const [bpm, setBpm] = useState("");
    const [durationSeconds, setDurationSeconds] = useState(0);
    const [submitError, setSubmitError] = useState("");
    const [submitting, setSubmitting] = useState(false);

    async function loadSession() {
        try {
            const headers = { Authorization: `Bearer ${token}` };
            const [sessionRes, participantsRes] = await Promise.all([
                fetch(`${API_BASE_URL}/api/sessions/${code}`, { headers }),
                fetch(`${API_BASE_URL}/api/sessions/${code}/participants`, { headers }),
            ]);

            if (!sessionRes.ok || !participantsRes.ok) {
                throw new Error("Failed to load session");
            }

            setSession(await sessionRes.json());
            setParticipants(await participantsRes.json());

            const submissionsRes = await fetch(`${API_BASE_URL}/api/sessions/${code}/submissions`, { headers });
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
        setGenre("");
        setKey("");
        setBpm("");
        setDurationSeconds(0);
        if (fileInputRef.current) fileInputRef.current.value = "";
    }

    async function handleSubmitSample(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        if (!file) return;

        setSubmitError("");
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

            resetUpload();
            loadSession();
        } catch (err) {
            console.error(err);
            setSubmitError("Could not submit sample.");
        } finally {
            setSubmitting(false);
        }
    }

    async function handleToggleSession() {
        if (!session) return;
        const action = session.isActive ? "close" : "reopen";

        const response = await fetch(`${API_BASE_URL}/api/sessions/${code}/${action}`, {
            method: "PUT",
            headers: { Authorization: `Bearer ${token}` },
        });

        if (response.ok) loadSession();
    }

    if (loading) return <div className="content"><p style={{ color: "var(--text-muted)" }}>Loading...</p></div>;
    if (error) return <div className="content"><p style={{ color: "var(--text-danger)" }}>{error}</p></div>;
    if (!session) return <div className="content"><p style={{ color: "var(--text-muted)" }}>Session not found.</p></div>;

    const isOwner = session.hostId === userId;

    return (
        <>
            <PageHeader
                title={session.name}
                backTo="/sessions"
                backLabel="Sessions"
                badge={
                    <span className={session.isActive ? "badge badge-green" : "badge badge-red"}>
            {session.isActive ? "Active" : "Closed"}
          </span>
                }
                actions={
                    <>
                        <button type="button" className="btn" onClick={() => navigator.clipboard.writeText(session.code)}>
                            <i className="ti ti-user-plus" /> Invite
                        </button>
                        {isOwner && (
                            <button
                                type="button"
                                className={session.isActive ? "btn btn-danger" : "btn"}
                                onClick={handleToggleSession}
                            >
                                <i className={session.isActive ? "ti ti-lock" : "ti ti-lock-open"} />
                                {session.isActive ? "Close session" : "Reopen session"}
                            </button>
                        )}
                    </>
                }
            />

            <div className="content">
                <div className="section-title" style={{ marginTop: 0 }}>Access code</div>
                <div className="code-box">
                    <div className="session-code">{session.code}</div>
                    <span className="badge badge-blue">
            {session.visibility === 1 ? "Collaborative" : "Curated"}
          </span>
                </div>

                <div className="grid-2">
                    <div>
                        <div className="section-title">Participants ({participants.length})</div>
                        <div className="card">
                            {participants.map((p) => (
                                <div key={p.id} className="participant">
                                    <div className="avatar">{p.userName[0]?.toUpperCase()}</div>
                                    <div style={{ fontSize: 13, flex: 1 }}>{p.userName}</div>
                                    {p.userId === session.hostId && <span className="badge badge-blue">Owner</span>}
                                </div>
                            ))}
                        </div>
                    </div>

                    <div>
                        <div className="section-title">Submissions ({submissions.length})</div>
                        <div className="card">
                            {submissions.length === 0 && (
                                <p style={{ color: "var(--text-muted)", fontSize: 13 }}>No submissions yet.</p>
                            )}
                            {submissions.map((s) => (
                                <div key={s.id} className="audio-file">
                                    <div className="play-btn"><i className="ti ti-music" style={{ fontSize: 13 }} /></div>
                                    <div className="audio-info">
                                        <div className="audio-name">{s.fileName}</div>
                                        <div className="audio-meta">{new Date(s.createdAt).toLocaleDateString()}</div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>

                <div className="section-title">Submit sample</div>
                {!file ? (
                    <div
                        className={`dropzone ${dragging ? "drag" : ""}`}
                        onClick={() => fileInputRef.current?.click()}
                        onDragOver={(e) => { e.preventDefault(); setDragging(true); }}
                        onDragLeave={() => setDragging(false)}
                        onDrop={handleDrop}
                    >
                        <i className="ti ti-upload" style={{ fontSize: 24, color: "var(--text-muted)" }} />
                        <div style={{ fontSize: 13, color: "var(--text-secondary)", marginTop: 8 }}>Drag a file here or</div>
                        <button type="button" className="btn" style={{ marginTop: 10 }}>Choose file</button>
                    </div>
                ) : (
                    <div className="card">
                        <form onSubmit={handleSubmitSample}>
                            <div className="audio-file">
                                <div className="play-btn"><i className="ti ti-music" style={{ fontSize: 13 }} /></div>
                                <div className="audio-info">
                                    <div className="audio-name">{file.name}</div>
                                    <div className="audio-meta">{durationSeconds}s</div>
                                </div>
                                <button type="button" className="btn" onClick={resetUpload}><i className="ti ti-x" /></button>
                            </div>

                            <div className="grid-4">
                                <div>
                                    <label htmlFor="fileName">Name</label>
                                    <input id="fileName" value={fileName} onChange={(e) => setFileName(e.target.value)} required />
                                </div>
                                <div>
                                    <label htmlFor="genre">Genre</label>
                                    <input id="genre" value={genre} onChange={(e) => setGenre(e.target.value)} required />
                                </div>
                                <div>
                                    <label htmlFor="key">Key</label>
                                    <input id="key" value={key} onChange={(e) => setKey(e.target.value)} required />
                                </div>
                                <div>
                                    <label htmlFor="bpm">BPM</label>
                                    <input id="bpm" type="number" value={bpm} onChange={(e) => setBpm(e.target.value)} required />
                                </div>
                            </div>

                            {submitError && <p style={{ color: "var(--text-danger)", fontSize: 13 }}>{submitError}</p>}
                            <button type="submit" className="btn btn-primary" disabled={submitting} style={{ marginTop: 12 }}>
                                {submitting ? "Submitting..." : "Submit sample"}
                            </button>
                        </form>
                    </div>
                )}

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

export default SessionDetail;