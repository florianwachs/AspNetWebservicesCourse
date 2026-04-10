import { Navbar } from "./components/Navbar";
import { EventList } from "./components/EventList";
import "./App.css";

function App() {
  return (
    <div className="app">
      <Navbar />
      <main className="main-content">
        <h1>TechConf Events</h1>
        <EventList />
      </main>
    </div>
  );
}

export default App;
