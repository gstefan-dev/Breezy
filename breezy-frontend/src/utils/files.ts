export async function computeHash(file: File): Promise<string> {
    const hashBuffer = await crypto.subtle.digest("SHA-256", await file.arrayBuffer());
    return Array.from(new Uint8Array(hashBuffer))
        .map((b) => b.toString(16).padStart(2, "0"))
        .join("");
}

export async function getAudioDuration(file: File): Promise<number> {
    const audioContext = new AudioContext();
    const audioBuffer = await audioContext.decodeAudioData(await file.arrayBuffer());
    return Math.round(audioBuffer.duration);
}