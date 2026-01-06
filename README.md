# GossipChat

A real-time chat application built with Vue.js frontend and .NET backend.

## Features

- Real-time messaging with SignalR
- User authentication and registration
- Group chats and private conversations
- Message attachments and replies
- Dark and light mode support
- Responsive design

## Technologies Used

- **Frontend:** Vue.js 3, TypeScript, Vite, Tailwind CSS
- **Backend:** .NET 8, ASP.NET Core, Entity Framework, SignalR
- **Database:** SQL Server (or as configured)

## Installation

### Prerequisites

- Node.js (v16 or higher)
- .NET 8 SDK
- SQL Server (or compatible database)

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/hoangquan2311/Gossip-ChatApp
   cd GossipChat
   ```

2. Install frontend dependencies:
   ```bash
   npm install
   ```

3. Setup the backend:
   - Navigate to `ChatService` directory
   - Restore NuGet packages:
     ```bash
     dotnet restore
     ```
   - Update the connection string in `appsettings.json`
   - Run database migrations:
     ```bash
     dotnet ef database update
     ```

4. Run the backend:
   ```bash
   dotnet run
   ```

5. Run the frontend (in a new terminal):
   ```bash
   npm run dev
   ```

6. Open your browser and navigate to `http://localhost:5173`

## Usage

- Register a new account or sign in
- Start a conversation with other users
- Create group chats
- Send messages, attachments, and replies

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## License

This project is licensed under the MIT License.
- <a href="https://pinia.vuejs.org/">Pinia</a>
- <a href="https://heroicons.com/">Heroicons</a>
- <a href="https://github.com/dcastil/tailwind-merge">Tailwind merge</a>
- <a href="https://vueuse.org/">vueuse</a>
- <a href="https://wavesurfer-js.org/">Wavesurfer-js</a>
- <a href="https://github.com/Akryum/floating-vue">floating-vue</a>
