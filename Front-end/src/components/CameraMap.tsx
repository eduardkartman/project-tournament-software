import { MapContainer, Marker, Popup, TileLayer } from "react-leaflet";
import "leaflet/dist/leaflet.css";
import type { Camera } from "../types/camera";
import { cameraMarkerIcon } from "./leafletIcon";

const UTRECHT_CENTER: [number, number] = [52.0914, 5.1115];
const INITIAL_ZOOM = 14;

interface CameraMapProps {
  cameras: Camera[];
}

export function CameraMap({ cameras }: CameraMapProps) {
  return (
    <MapContainer
      center={UTRECHT_CENTER}
      zoom={INITIAL_ZOOM}
      className="camera-map"
      scrollWheelZoom
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      {cameras.map((camera) => (
        <Marker
          key={camera.number}
          position={[camera.latitude, camera.longitude]}
          icon={cameraMarkerIcon}
          title={camera.name}
        >
          <Popup>{camera.name}</Popup>
        </Marker>
      ))}
    </MapContainer>
  );
}
