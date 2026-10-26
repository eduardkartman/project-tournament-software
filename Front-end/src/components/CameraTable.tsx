import type { CameraColumn } from "../domain/cameraColumns";

interface CameraTableProps {
  columns: CameraColumn[];
}

export function CameraTable({ columns }: CameraTableProps) {
  return (
    <div className="camera-table">
      {columns.map((column) => (
        <section
          key={column.id}
          className="camera-column"
          aria-labelledby={`heading-${column.id}`}
        >
          <h2 id={`heading-${column.id}`}>
            {column.title}{" "}
            <span className="count">({column.cameras.length})</span>
          </h2>
          <ul>
            {column.cameras.map((camera) => (
              <li key={camera.number}>{camera.name}</li>
            ))}
          </ul>
        </section>
      ))}
    </div>
  );
}
