function openTab(tabId) {
  document.querySelectorAll(".tab").forEach(tab => tab.classList.remove("active"));
  document.getElementById(tabId).classList.add("active");
  document.querySelectorAll("nav button").forEach(btn => btn.classList.remove("active"));
  event.target.classList.add("active");
}

function sendMessage() {
  const input = document.getElementById("chatInput");
  const chatbox = document.getElementById("chatbox");
  if(input.value.trim() !== ""){
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
function login() {
  const user = document.getElementById("loginUser").value;
  const pass = document.getElementById("loginPass").value;
  alert("Đăng nhập thành công!\nTài khoản: " + user);
  closeAuth();
}
function register() {
  const user = document.getElementById("regUser").value;
  const pass = document.getElementById("regPass").value;
  alert("Đăng ký thành công!\nTài khoản: " + user);
  closeAuth();
}

let slideIndex = 1;
showSlides(slideIndex);

function plusSlides(n) {
  showSlides(slideIndex += n);
}
function showSlides(n) {
  let slides = document.getElementsByClassName("slide");
  if (slides.length === 0) return;
  if (n > slides.length) {slideIndex = 1}
  if (n < 1) {slideIndex = slides.length}
  for (let i = 0; i < slides.length; i++) {
    slides[i].style.display = "none";
  }
  slides[slideIndex-1].style.display = "block";
}

// Tự động chuyển slide mỗi 5 giây
setInterval(() => {
  plusSlides(1);
}, 5000);
// Nghe phát âm từ
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

// Lưu từ vào danh sách của người dùng
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
// Ghi âm phát âm và gửi tới API chấm điểm
async function recordSpeech() {
  alert("Demo: chức năng ghi âm cần WebRTC hoặc MediaRecorder API.");
  // Sau khi ghi âm xong => gửi file tới API
  // let formData = new FormData();
  // formData.append("audio", blob);
  // await fetch("http://localhost:5000/api/speech/check", { method: "POST", body: formData });
}
// Kiểm tra ngữ pháp từ textarea
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
// Sinh câu hỏi trắc nghiệm từ đoạn văn
async function generateQuestions() {
  const res = await fetch("http://localhost:5000/api/reading/questions", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ text: "Lorem ipsum..." }) // lấy text thực tế từ UI
  });

  if (res.ok) {
    const data = await res.json();
    alert("Sinh câu hỏi: " + JSON.stringify(data));
  } else {
    alert("Không tạo được câu hỏi!");
  }
}
// Gửi đáp án của người dùng
async function submitAnswer(answer) {
  const res = await fetch("http://localhost:5000/api/exam/submit", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ questionId: 1, answer }) // thêm questionId thật
  });

  if (res.ok) {
    const data = await res.json();
    alert("Kết quả: " + data.correct ? "Đúng" : "Sai");
  } else {
    alert("Không gửi được đáp án!");
  }
}
// Lấy dữ liệu tiến độ từ backend
async function loadProgress() {
  const res = await fetch("http://localhost:5000/api/progress");
  if (res.ok) {
    const data = await res.json();
    // Gán dữ liệu vào progress bar
    document.querySelector("#progress progress:nth-of-type(1)").value = data.vocab;
    document.querySelector("#progress progress:nth-of-type(2)").value = data.listening;
    document.querySelector("#progress progress:nth-of-type(3)").value = data.writing;
  }
}
// Hiện modal đăng nhập / đăng ký
function showAuth(type) {
  document.getElementById("authModal").style.display = "block";
  document.getElementById("loginForm").style.display = type === "login" ? "block" : "none";
  document.getElementById("registerForm").style.display = type === "register" ? "block" : "none";
}

// Đóng modal
function closeAuth() {
  document.getElementById("authModal").style.display = "none";
}

// Xử lý đăng nhập
async function login() {
  const user = document.getElementById("loginUser").value.trim();
  const pass = document.getElementById("loginPass").value.trim();

  if (!user || !pass) {
    alert("⚠️ Vui lòng nhập đủ tên đăng nhập và mật khẩu.");
    return;
  }

  try {
    // Giả lập API
    console.log("Login request:", { user, pass });
    // Sau này thay bằng:
    // let res = await fetch("/api/auth/login", {
    //   method: "POST",
    //   headers: { "Content-Type": "application/json" },
    //   body: JSON.stringify({ user, pass })
    // });
    // let data = await res.json();

    alert("✅ Đăng nhập thành công!\nXin chào " + user);
    closeAuth();
  } catch (err) {
    alert("❌ Lỗi khi đăng nhập!");
    console.error(err);
  }
}

// Xử lý đăng ký
async function register() {
  const user = document.getElementById("regUser").value.trim();
  const pass = document.getElementById("regPass").value.trim();

  if (!user || !pass) {
    alert("⚠️ Vui lòng nhập đủ thông tin đăng ký.");
    return;
  }

  try {
    // Giả lập API
    console.log("Register request:", { user, pass });
    // Sau này thay bằng:
    // let res = await fetch("/api/auth/register", {
    //   method: "POST",
    //   headers: { "Content-Type": "application/json" },
    //   body: JSON.stringify({ user, pass })
    // });
    // let data = await res.json();

    alert("🎉 Đăng ký thành công!\nTài khoản: " + user);
    closeAuth();
  } catch (err) {
    alert("❌ Lỗi khi đăng ký!");
    console.error(err);
  }
}

