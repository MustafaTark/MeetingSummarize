let connection;
let pc;
let localStream;
let mediaRecorder;

async function init() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl('/hubs/meeting')
        .withAutomaticReconnect()
        .build();

    connection.on('Signal', async (msg) => {
        const { type, payload, from } = msg;
        if (!pc) return;
        if (type === 'offer') {
            await pc.setRemoteDescription(new RTCSessionDescription(JSON.parse(payload)));
            const answer = await pc.createAnswer();
            await pc.setLocalDescription(answer);
            await connection.invoke('SendSignal', window.__meetingId, 'answer', JSON.stringify(answer));
        } else if (type === 'answer') {
            await pc.setRemoteDescription(new RTCSessionDescription(JSON.parse(payload)));
        } else if (type === 'candidate') {
            await pc.addIceCandidate(new RTCIceCandidate(JSON.parse(payload)));
        }
    });

    connection.on('ChatMessage', (msg) => {
        const el = document.createElement('div');
        el.textContent = `${msg.from}: ${msg.message}`;
        document.getElementById('chat').appendChild(el);
    });

    await connection.start();
    await connection.invoke('JoinRoom', window.__meetingId);
}

async function start() {
    localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
    document.getElementById('localVideo').srcObject = localStream;

    pc = new RTCPeerConnection();
    pc.ontrack = (ev) => {
        document.getElementById('remoteVideo').srcObject = ev.streams[0];
    };
    pc.onicecandidate = async (ev) => {
        if (ev.candidate) {
            await connection.invoke('SendSignal', window.__meetingId, 'candidate', JSON.stringify(ev.candidate));
        }
    };

    localStream.getTracks().forEach(t => pc.addTrack(t, localStream));

    const offer = await pc.createOffer();
    await pc.setLocalDescription(offer);
    await connection.invoke('SendSignal', window.__meetingId, 'offer', JSON.stringify(offer));

    // Start audio recording chunks for transcription
    try {
        const audioStream = new MediaStream(localStream.getAudioTracks());
        // اختر نوع ترميز مدعوم إن توفر
        const candidates = ['audio/webm;codecs=opus', 'audio/webm', 'audio/ogg;codecs=opus'];
        const supported = (window.MediaRecorder && typeof MediaRecorder.isTypeSupported === 'function')
            ? candidates.find(t => MediaRecorder.isTypeSupported(t))
            : undefined;
        mediaRecorder = supported
            ? new MediaRecorder(audioStream, { mimeType: supported })
            : new MediaRecorder(audioStream);
        mediaRecorder.ondataavailable = async (e) => {
            if (e.data && e.data.size > 0) {
                // طبّع نوع الـ Blob لحذف أي معاملات (مثل codecs) حتى يكون Content-Type نظيفًا
                const cleanType = (e.data.type && e.data.type.includes(';'))
                    ? e.data.type.split(';')[0]
                    : (e.data.type || 'audio/webm');
                const normalizedBlob = new Blob([e.data], { type: cleanType });

                const form = new FormData();
                form.append('audio', normalizedBlob, 'chunk.webm');
                await fetch(`/api/Transcription/${window.__meetingId}/chunk`, {
                    method: 'POST',
                    body: form
                });
            }
        };
        mediaRecorder.start(5000); // chunk every 5s
    } catch (err) {
        console.warn('MediaRecorder not available', err);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    init();
    document.getElementById('chatSend').addEventListener('click', async () => {
        const input = document.getElementById('chatInput');
        const msg = input.value;
        input.value = '';
        await connection.invoke('SendChatMessage', window.__meetingId, msg);
    });
    document.getElementById('startBtn').addEventListener('click', start);
});


