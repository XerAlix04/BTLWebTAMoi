function openTab(tabId) {
    document.querySelectorAll(".tab").forEach(tab => tab.classList.remove("active"));
    document.getElementById(tabId).classList.add("active");
    document.querySelectorAll("nav button").forEach(btn => btn.classList.remove("active"));
    event.target && event.target.classList.add("active");
}

// Make openTab available globally for inline onclick
window.openTab = openTab;


function sendMessage() {
    const input = document.getElementById("chatInput");
    const chatbox = document.getElementById("chatbox");

    if (input.value.trim() !== "") {
        chatbox.innerHTML += `<p><b>You:</b> ${input.value}</p>`;
        chatbox.innerHTML += `<p><b>Bot:</b> (AI trả lời ở đây)</p>`;
        input.value = "";
        chatbox.scrollTop = chatbox.scrollHeight;
    }
}


function showAuth(type) {
    document.getElementById("authModal").style.display = "block";
    document.getElementById("loginForm").style.display = type === "login" ? "block" : "none";
    document.getElementById("registerForm").style.display = type === "register" ? "block" : "none";
}

function closeAuth() {
    document.getElementById("authModal").style.display = "none";
}

async function login() {
    const user = document.getElementById("loginUser").value.trim();
    const pass = document.getElementById("loginPass").value.trim();

    if (!user || !pass) {
        alert("⚠️ Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.");
        return;
    }

    try {
        const body = {
            TenDangNhap: user,
            MatKhau: pass
        };

        const response = await fetch("/Auth/Login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(body)
        });

        const result = await response.json();

        if (!response.ok) {
            alert("❌ " + (result.message || "Đăng nhập thất bại!"));
            return;
        }

        alert("✅ đc r " + result.message + "\nXin chào " + result.user.TenDangNhap);

        document.getElementById("userNameDisplay").textContent = result.user.TenDangNhap;

        sessionStorage.setItem("userName", result.user.TenDangNhap);
        sessionStorage.setItem("userId", result.user.MaNguoiDung);

        closeAuth();
    } catch (err) {
        console.error("Lỗi khi đăng nhập:", err);
        alert("⚠️ Đã xảy ra lỗi kết nối tới server!");
    }
}


async function register() {
    console.log("✅ Hàm register() đã được gọi!");
    const username = document.getElementById("regUser").value.trim();
    const email = document.getElementById("regEmail").value.trim();
    const password = document.getElementById("regPass").value.trim();

    if (!username || !email || !password) {
        alert("⚠️ Vui lòng nhập đầy đủ Tên đăng nhập, Email và Mật khẩu zô zô.");
        return;
    }

    try {
        const res = await fetch("https://localhost:7290/Auth/Register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                TenDangNhap: username,
                Email: email,
                MatKhau: password
            })
        });

        if (res.ok) {
            const data = await res.json();
            if (data.success) {
                alert("🎉 " + data.message + "\nChào mừng " + data.user.tenDangNhap + "!");
                closeAuth();
            } else {
                alert("⚠️ " + data.message);
            }
        } else {
            const err = await res.json();
            alert("❌ Lỗi khi đăng ký: " + (err.message || "Không xác định"));
        }


    } catch (error) {
        console.error("Lỗi fetch:", error);
        alert("❌ Không thể kết nối tới server!");
    }
}



/// Phát âm
async function playAudio(word) {
    const res = await fetch(`http://localhost:5000/api/vocab/pronounce?word=${word}`);
    if (res.ok) {
        const blob = await res.blob();
        const url = URL.createObjectURL(blob);
        new Audio(url).play();
    } else {
        alert("Không phát được âm thanh!");
    }
}

// Lưu từ vựng
async function saveWord(word) {
    const res = await fetch("http://localhost:5000/api/vocab/save", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ word })
    });
    if (res.ok) {
        alert("Đã lưu từ: " + word);
    } else {
        alert("Không thể lưu từ!");
    }
}

// Ghi âm phát âm
document.addEventListener("DOMContentLoaded", () => {
    const startBtn = document.getElementById("startRecordingBtn");
    const stopBtn = document.getElementById("stopRecordingBtn");
    const audioPlayer = document.getElementById("audioPlayer");
    const statusDiv = document.getElementById("speechStatus");
    const resultDiv = document.getElementById("speechResult");

    let mediaRecorder;
    let audioChunks = [];

    startBtn.addEventListener("click", async () => {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            mediaRecorder = new MediaRecorder(stream, { mimeType: "audio/webm" });
            audioChunks = [];

            mediaRecorder.ondataavailable = e => audioChunks.push(e.data);

            mediaRecorder.onstop = async () => {
                const audioBlob = new Blob(audioChunks, { type: 'audio/webm' });
                audioPlayer.src = URL.createObjectURL(audioBlob);
                audioPlayer.style.display = "block";

                statusDiv.textContent = "⏳ Đang gửi file lên server...";
                const formData = new FormData();
                formData.append("audio", audioBlob, "recorded.webm");

                try {
                    const response = await fetch("/Speech/Check", { method: "POST", body: formData });
                    const html = await response.text();
                    resultDiv.innerHTML = html;
                    statusDiv.textContent = "✅ Gửi thành công!";
                } catch (err) {
                    console.error("Upload failed:", err);
                    statusDiv.textContent = "❌ Gửi thất bại!";
                }

                stopBtn.disabled = true;
                startBtn.disabled = false;
            };

            mediaRecorder.start();
            startBtn.disabled = true;
            stopBtn.disabled = false;
            statusDiv.textContent = "🎤 Đang ghi âm...";
            resultDiv.innerHTML = "";
        } catch (err) {
            console.error("Mic error:", err);
            statusDiv.textContent = "❌ Không truy cập được micro!";
        }
    });

    stopBtn.addEventListener("click", () => {
        if (mediaRecorder && mediaRecorder.state !== "inactive") {
            mediaRecorder.stop();
            statusDiv.textContent = "⏹️ Đã dừng ghi âm, đang xử lý...";
        } else {
            statusDiv.textContent = "⚠️ Không có ghi âm nào đang chạy!";
        }
    });
});


// Kiểm tra ngữ pháp
async function checkGrammar() {
    const text = document.querySelector("#writing textarea").value;
    if (!text.trim()) return alert("Nhập văn bản trước!");

    const res = await fetch("http://localhost:5000/api/writing/check", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ text })
    });

    if (res.ok) {
        const data = await res.json();
        alert("Kết quả: " + data.corrections);
    } else {
        alert("Không kiểm tra được ngữ pháp!");
    }
}

// Sinh câu hỏi trắc nghiệm
async function generateQuestions() {
    const res = await fetch("http://localhost:5000/api/reading/questions", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ text: "Lorem ipsum..." })
    });

    if (res.ok) {
        const data = await res.json();
        alert("Sinh câu hỏi: " + JSON.stringify(data));
    } else {
        alert("Không tạo được câu hỏi!");
    }
}

// Gửi đáp án (per-question API; preserved for compatibility)
async function submitAnswer(answer) {
    const res = await fetch("http://localhost:5000/api/exam/submit", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ questionId: 1, answer })
    });

    if (res.ok) {
        const data = await res.json();
        alert("Kết quả: " + (data.correct ? "Đúng" : "Sai"));
    } else {
        alert("Không gửi được đáp án!");
    }
}


async function loadProgress() {
    const res = await fetch("http://localhost:5000/api/progress");
    if (res.ok) {
        const data = await res.json();
        document.querySelector("#progress progress:nth-of-type(1)").value = data.vocab;
        document.querySelector("#progress progress:nth-of-type(2)").value = data.listening;
        document.querySelector("#progress progress:nth-of-type(3)").value = data.writing;
    }
}

window.addEventListener("DOMContentLoaded", function() {
    const userName = sessionStorage.getItem("userName");
    if (userName) {
        const el = document.getElementById("userNameDisplay");
        if (el) el.textContent = userName;
    }
});
async function uploadAndRecognize() {
    const fileInput = document.getElementById("ocrFile");
    const preview = document.getElementById("previewImage");
    const resultEl = document.getElementById("ocrResult");
    const status = document.getElementById("ocrStatus");

    const file = fileInput.files[0];
    if (!file) {
        alert("Vui lòng chọn ảnh để nhận diện.");
        return;
    }

    // Hiển thị ảnh preview
    const url = URL.createObjectURL(file);
    preview.src = url;
    preview.style.display = "block";

    // Reset trạng thái
    resultEl.textContent = "";
    status.textContent = "⏳ Đang gửi ảnh lên...";

    try {
        // Chuẩn bị form
        const formData = new FormData();
        formData.append("file", file);

        // Gọi đến backend ASP.NET MVC
        const response = await fetch("/Ocr/Recognize", {
            method: "POST",
            body: formData
        });

        if (!response.ok) {
            const errText = await response.text();
            throw new Error(errText);
        }

        const data = await response.json();

        if (data.recognized_text) {
            resultEl.textContent = data.recognized_text;
            status.textContent = "✅ Nhận diện thành công!";
        } else {
            status.textContent = "⚠️ Không có văn bản nhận được.";
            resultEl.textContent = JSON.stringify(data, null, 2);
        }

    } catch (err) {
        console.error(err);
        status.textContent = "❌ Lỗi khi nhận diện: " + err.message;
    } finally {
        // Giải phóng blob preview
        URL.revokeObjectURL(url);
    }
}

document.getElementById("btnOcr")?.addEventListener("click", uploadAndRecognize);

// --- QUIZ / EXAM FLOW IMPLEMENTATION ---

// Quiz state
let quizQuestions = []; // each { id, question, choices: {A,B,C,D}, correct: 'A' }
let currentIndex = 0;
let totalQuestions = 10;
let correctCount = 0;
let answered = false;
let autoAdvanceTimeout = 800; // ms

// Expose functions globally for inline handlers
window.startQuiz = startQuiz;
window.selectAnswer = selectAnswer;
window.nextQuestion = nextQuestion;

async function startQuiz() {
    currentIndex = 0;
    correctCount = 0;
    answered = false;
    document.getElementById("quizResult").style.display = "none";
    document.getElementById("nextBtn").style.display = "none";
    document.getElementById("startQuizBtn").style.display = "none";
    document.getElementById("progressInfo").textContent = "";

    try {
        // ✅ Gọi API backend để lấy câu hỏi
        const res = await fetch(`https://localhost:7290/api/Exam/GetQuestions?count=${totalQuestions}`);
        if (!res.ok) throw new Error("Không thể tải câu hỏi từ server");

        const data = await res.json();

        // ✅ Chuẩn hóa dữ liệu cho frontend
        quizQuestions = data.map(q => ({
            id: q.ma_cau,
            question: q.noi_dung,
            choices: q.luaChon.reduce((acc, lc, idx) => {
                const letter = String.fromCharCode(65 + idx); // A,B,C,D...
                acc[letter] = lc.noi_dung;
                if (lc.la_dap_an) acc.correct = letter;
                return acc;
            }, {})
        }));

        // Cắt giới hạn 10 câu
        quizQuestions = quizQuestions.slice(0, totalQuestions);

    } catch (err) {
        console.error("❌ Lỗi khi tải câu hỏi:", err);
        alert("Không thể tải câu hỏi từ server, vui lòng thử lại sau.");
        return;
    }

    document.getElementById("totalQuestions").textContent = totalQuestions;

    document.querySelectorAll(".answer-btn").forEach(btn => {
        btn.disabled = false;
        btn.classList.remove("correct", "incorrect", "selected");
    });

    loadQuestion();
}

function loadQuestion() {
    answered = false;
    const q = quizQuestions[currentIndex];
    if (!q) return finishQuiz();

    document.getElementById("questionNumber").textContent = currentIndex + 1;
    document.getElementById("examQuestion").textContent = q.question;

    const buttons = document.querySelectorAll(".answer-btn");
    buttons.forEach(btn => {
        const choice = btn.getAttribute("data-choice");
        btn.textContent = `${choice}. ${q.choices[choice] || ''}`;
        btn.disabled = false;
        btn.classList.remove("correct", "incorrect", "selected");
    });

    document.getElementById("nextBtn").style.display = "none";
    document.getElementById("progressInfo").textContent = `Đã đúng ${correctCount} / ${currentIndex} câu`;
}

function selectAnswer(choice) {
    if (answered) return;
    answered = true;

    const q = quizQuestions[currentIndex];
    const correct = q.choices.correct;

    document.querySelectorAll(".answer-btn").forEach(btn => {
        const c = btn.getAttribute("data-choice");
        btn.disabled = true;
        if (c === correct) btn.classList.add("correct");
        if (c === choice && c !== correct) btn.classList.add("incorrect");
    });

    if (choice === correct) correctCount++;

    document.getElementById("progressInfo").textContent = `Đã đúng ${correctCount} / ${currentIndex + 1} câu`;

    if (currentIndex < totalQuestions - 1) {
        setTimeout(() => {
            currentIndex++;
            loadQuestion();
        }, autoAdvanceTimeout);
    } else {
        setTimeout(() => finishQuiz(), autoAdvanceTimeout);
    }
}

function nextQuestion() {
    if (currentIndex < totalQuestions - 1) {
        currentIndex++;
        loadQuestion();
    } else {
        finishQuiz();
    }
}

async function finishQuiz() {
    document.querySelectorAll(".answer-btn").forEach(btn => btn.disabled = true);
    document.getElementById("quizResult").style.display = "block";
    document.getElementById("resultText").textContent = `Bạn đúng ${correctCount} trên ${totalQuestions} câu.`;

    // ✅ Gửi kết quả về backend
    try {
        const userId = sessionStorage.getItem("userId") || null;
        await fetch("https://localhost:7290/api/Exam/SubmitQuiz", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId,
                total: totalQuestions,
                correct: correctCount,
                details: quizQuestions.map(q => ({
                    id: q.id,
                    question: q.question
                }))
            })
        });
    } catch (e) {
        console.warn("Gửi kết quả thất bại:", e);
    }

    document.getElementById("startQuizBtn").style.display = "inline-block";
    document.getElementById("nextBtn").style.display = "none";
}


// --- End quiz flow implementation ---


// --- HIỂN THỊ VÀ LẤY TỪ RANDOM TỪ BACKEND ---MOI
async function loadRandomWord() {
    try {
        const res = await fetch("https://localhost:7290/Speech/Index", {
            method: "GET",
            headers: { "Accept": "application/json" }
        });

        if (!res.ok) throw new Error("Không tải được từ.");
        const data = await res.json();
        document.getElementById("randomWord").textContent = data.randomWord;

        // Lưu lại để gửi khi chấm điểm
        window.currentWord = data.randomWord;

    } catch (err) {
        document.getElementById("randomWord").textContent = "❌ Lỗi tải từ";
        console.error(err);
    }
}

const btn = document.getElementById("newWordBtn");
if (btn) btn.addEventListener("click", loadRandomWord);

// Tải từ đầu tiên khi mở tab
loadRandomWord();
