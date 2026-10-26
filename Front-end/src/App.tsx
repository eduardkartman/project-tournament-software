import { CameraMap } from "./components/CameraMap";
import { CameraTable } from "./components/CameraTable";
import { groupCameras } from "./domain/cameraColumns";
import { useCameras } from "./hooks/useCameras";
import "./App.css";

export default function App() {
  const state = useCameras();

  return (
    <main className="app">
      <h1>Utrecht cameras</h1>

      {state.status === "loading" && <p>Loading cameras…</p>}
      {state.status === "error" && (
        <p role="alert" className="error">
          Could not load the cameras: {state.message} Is the API running?
        </p>
      )}
      {state.status === "success" && (
        <>
          <CameraMap cameras={state.cameras} />
          <CameraTable columns={groupCameras(state.cameras)} />
        </>
      )}
    </main>
  );
}
