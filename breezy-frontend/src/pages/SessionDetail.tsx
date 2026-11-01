import { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { API_BASE_URL } from "../config/api";
import PageHeader from "../components/layout/PageHeader";
import { getAudioDuration } from "../utils/files";

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
    leftAt: string | null;
    exitReason: "Left" | "RemovedByOwner" | null;
}

interface Submission {
    id: string;
    submitterId: string;
    fileName: string;
    fileUrl: string;
    createdAt: string;
}

function SessionDetail() {
    const { code } = useParams();
    const { token, userId } = useAuth();
    const navigate = useNavigate();
    const [session, setSession] = useState<Session | null>(null);
    const [participants, setParticipants] = useState<Participant[]>([]);
    const [submissions, setSubmissions] = useState<Submission[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [actionError, setActionError] = useState("");

    const [confirmLeave, setConfirmLeave] = useState(false);
    const [confirmRemoveId, setConfirmRemoveId] = useState<string | null>(null);

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

    const headers = { Authorization: `Bearer ${token}` };

    async function loadSession() {
        try {
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

    async function postAction(url: string, fallbackError: string) {
        setActionError("");
        const response = await fetch(url, { method: "POST", headers });

        if (!response.ok) {
            const body = await response.json().catch(() => null);
            setActionError(body?.error ?? fallbackError);
            return false;
        }
        return true;
    }

    async function handleToggleSession() {
        if (!session) return;
        const action = session.isActive ? "close" : "reopen";

        const response = await fetch(`${API_BASE_URL}/api/sessions/${code}/${action}`, { method: "PUT", headers });
        if (response.ok) loadSession();
    }

    async function handleLeave() {
        const ok = await postAction(`${API_BASE_URL}/api/sessions/${code}/leave`, "Could not leave session.");
        if (ok) navigate("/sessions");
    }

    async function handleRemove(participantUserId: string) {
        const ok = await postAction(
            `${API_BASE_URL}/api/sessions/${code}/participants/${participantUserId}/remove`,
            "Could not remove participant."
        );
        setConfirmRemoveId(null);
        if (ok) loadSession();
    }

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
                headers,
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

    if (loading) return <div className="content"><p style={{ color: "var(--text-muted)" }}>Loading...</p></div>;
    if (error) return <div className="content"><p style={{ color: "var(--text-danger)" }}>{error}</p></div>;
    if (!session) return <div className="content"><p style={{ color: "var(--text-muted)" }}>Session not found.</p></div>;

    const isOwner = session.hostId === userId;
    const me = participants.find((p) => p.userId === userId);
    const isMember = !!me && me.leftAt === null;
    const activeParticipants = participants.filter((p) => p.leftAt === null);
    const formerParticipants = participants.filter((p) => p.leftAt !== null);

    return (
        <>
            <PageHeader
                title={session.name}
                backTo="/sessions"
                backLabel="Sessions"
                badge={
                    <span className={session.isActive ? "badge badge-green" : "badge badge-red"}>
            {session.isActive ? "Open" : "Closed"}
          </span>
                }
                actions={
                    <>
                        {isMember && (
                            <button type="button" className="btn" onClick={() => navigator.clipboard.writeText(session.code)}>
                                <i className="ti ti-user-plus" /> Invite
                            </button>
                        )}

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

                        {!isOwner && isMember && !confirmLeave && (
                            <button type="button" className="btn" onClick={() => setConfirmLeave(true)}>
                                <i className="ti ti-logout" /> Leave session
                            </button>
                        )}

                        {!isOwner && isMember && confirmLeave && (
                            <>
                                <button type="button" className="btn" onClick={() => setConfirmLeave(false)}>Cancel</button>
                                <button type="button" className="btn btn-danger" onClick={handleLeave}>
                                    <i className="ti ti-logout" /> Confirm leave
                                </button>
                            </>
                        )}
                    </>
                }
            />

            <div className="content">
                {actionError && <p style={{ color: "var(--text-danger)", fontSize: 13, marginBottom: 10 }}>{actionError}</p>}

                {me && !isMember && (
                    <div className="card" style={{ background: "var(--bg-danger)", borderColor: "transparent" }}>
                        <p style={{ color: "var(--text-danger)", fontSize: 13 }}>
                            {me.exitReason === "RemovedByOwner"
                                ? "You were removed from this session by the owner."
                                : "You left this session. You can rejoin with the code while it is open."}
                        </p>
                    </div>
                )}

                <div className="section-title" style={{ marginTop: 0 }}>Access code</div>
                <div className="code-box">
                    <div className="session-code">{session.code}</div>
                    <span className="badge badge-blue">
            {session.visibility === 1 ? "Collaborative" : "Curated"}
          </span>
                </div>

                <div className="grid-2">
                    <div>
                        <div className="section-title">Participants ({activeParticipants.length})</div>
                        <div className="card">
                            {activeParticipants.map((p) => {
                                const isHost = p.userId === session.hostId;
                                const canRemove = isOwner && !isHost;

                                return (
                                    <div key={p.id} className="participant">
                                        <div className="avatar">{p.userName[0]?.toUpperCase()}</div>
                                        <div style={{ fontSize: 13, flex: 1 }}>{p.userName}</div>

                                        {isHost && <span className="badge badge-blue">Owner</span>}

                                        {canRemove && confirmRemoveId !== p.userId && (
                                            <button type="button" className="btn" onClick={() => setConfirmRemoveId(p.userId)}>
                                                <i className="ti ti-user-minus" />
                                            </button>
                                        )}

                                        {canRemove && confirmRemoveId === p.userId && (
                                            <>
                                                <button type="button" className="btn" onClick={() => setConfirmRemoveId(null)}>Cancel</button>
                                                <button type="button" className="btn btn-danger" onClick={() => handleRemove(p.userId)}>
                                                    Remove
                                                </button>
                                            </>
                                        )}
                                    </div>
                                );
                            })}
                        </div>

                        {formerParticipants.length > 0 && (
                            <>
                                <div className="section-title">Former participants ({formerParticipants.length})</div>
                                <div className="card">
                                    {formerParticipants.map((p) => (
                                        <div key={p.id} className="participant" style={{ opacity: 0.7 }}>
                                            <div className="avatar">{p.userName[0]?.toUpperCase()}</div>
                                            <div style={{ fontSize: 13, flex: 1 }}>{p.userName}</div>
                                            <span style={{ fontSize: 11, color: "var(--text-muted)" }}>
                        {p.exitReason === "RemovedByOwner" ? "Removed by owner" : "Left"} on{" "}
                                                {new Date(p.leftAt!).toLocaleDateString()}
                      </span>
                                        </div>
                                    ))}
                                </div>
                            </>
                        )}
                    </div>

                    <div>
                        <div className="section-title">Submissions ({submissions.length})</div>
                        <div className="card">
                            {submissions.length === 0 && (
                                <p style={{ color: "var(--text-muted)", fontSize: 13 }}>
                                    {isMember ? "No submissions yet." : "Submissions are visible only to active participants."}
                                </p>
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

                {isMember && session.isActive && (
                    <>
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
                                    <button type="submit" disabled={submitting} style={{ marginTop: 12 }}>
                                        {submitting ? "Submitting..." : "Submit sample"}
                                    </button>
                                </form>
                            </div>
                        )}
                    </>
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