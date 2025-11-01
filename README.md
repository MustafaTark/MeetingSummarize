# Meeting Summarizer Agent (Proof of Concept)

## Overview
**Meeting Summarizer Agent** is an MVC **.NET** proof-of-concept project that demonstrates how to integrate AI-driven meeting analysis into a traditional web application.

The application allows you to open a video call or meeting, capture the **audio stream**, and use **OpenAI’s Speech-to-Text** and **Text Analysis APIs** to summarize the discussion in real time.  

At the end of each session, it automatically generates a **concise meeting summary**, highlighting:
- Key discussion points  
- Important decisions  
- Action items  

---

## Features
- 🎙️ **Real-Time Audio Capture** – Streams meeting audio directly from the browser or input source.  
- 🧠 **AI-Powered Analysis** – Uses OpenAI APIs for transcription and intelligent summarization.  
- 🗂️ **Topic & Decision Extraction** – Identifies key topics, agreements, and next steps.  
- 📅 **Actionable Insights (Planned)** – Will soon create Google Calendar events automatically for confirmed schedules or action items via **MCP integration**.  

---

## Technology Stack
- **Backend:** ASP.NET MVC (.NET 8)  
- **Frontend:** Razor Views, JavaScript (WebRTC for audio streaming)  
- **AI Integration:** OpenAI Whisper (Speech-to-Text) + GPT-based summarization  
- **Planned Integration:** Google Calendar API (via MCP)  

---

## Architecture
The system follows a **modular architecture** that separates:
- **Audio processing**
- **AI interaction services**
- **Summary generation**
- **External API integration**

This design makes it easier to extend or replace components—for example, switching from OpenAI to another provider or adding new automation agents.

---

## Future Enhancements
- ✅ Automate event creation for scheduled meetings  
- ✅ Add persistent meeting history and summaries  
- ✅ Enable team-based access and sharing  
- ✅ Enhance summarization with speaker recognition and sentiment analysis  

---

## Key Learnings
This project explores how **AI-powered language understanding** can enhance traditional .NET web apps.  
It focuses on:
- Integrating **OpenAI services** for real-time processing  
- Building **autonomous workflow agents** using APIs  
- Designing **scalable, modular** components for hybrid AI + web systems  

---

## Getting Started
1. **Clone the repository**
   ```bash
   git clone https://github.com/MustafaTark/meeting-summarizer-agent.git
