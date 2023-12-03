import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit, getModuleFactory } from '@angular/core';
import * as mapboxgl from 'mapbox-gl';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit {
  private map: mapboxgl.Map | undefined;
  private pointId: number = 0;
  protected widthInDeg: number = 0.05;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.map = new mapboxgl.Map({
      accessToken: environment.mapbox.accessToken,
      container: 'mapContainer',
      style: 'mapbox://styles/mapbox/streets-v11',
      zoom: 13,
      center: [17.107748, 48.148598],
    });

    this.restoreMapCamera();

    this.map.addControl(new mapboxgl.NavigationControl());
    this.map.on('load', () => {
      if (this.map == undefined) return;

      this.map.addSource('pointSource', {
        type: 'geojson',
        data: {
          type: 'Feature',
          geometry: {
            type: 'Point',
            coordinates: [17.107748, 48.148598],
          },
          properties: {
            name: 'point',
          },
        },
      });

      this.map.addSource('lineSource', {
        type: 'geojson',
        data: {
          type: 'Feature',
          geometry: {
            type: 'LineString',
            coordinates: [
              [17.107748, 48.148598],
              [17.107748, 48.158598],
              [17.117748, 48.158598],
              [17.137748, 48.148598],
            ],
          },
          properties: {
            name: 'LineString',
          },
        },
      });

      this.map.addSource('polygonSource', {
        type: 'geojson',
        data: {
          type: 'Feature',
          geometry: {
            type: 'Polygon',
            coordinates: [
              [
                [17.107748, 48.148598],
                [17.107748, 48.158598],
                [17.117748, 48.158598],
                [17.117748, 48.148598],
                [17.107748, 48.148598],
              ],
            ],
          },
          properties: {
            name: 'Polygon',
          },
        },
      });

      this.map.addLayer({
        id: 'pointLayer',
        type: 'circle',
        source: 'pointSource',
        paint: {
          'circle-radius': {
            base: 5,
            stops: [[1, 5]],
          },
          'circle-color': '#e55e5e',
        },
      });

      this.map.loadImage(
        'https://docs.mapbox.com/mapbox-gl-js/assets/custom_marker.png',
        (error, image) => {
          if (error || this.map == undefined || image == undefined) {
            return;
          }

          this.map.addImage('custom-marker', image);
          this.map.addLayer({
            id: 'markerLayer',
            type: 'symbol',
            source: 'pointSource',
            layout: {
              'icon-image': 'custom-marker',
              'icon-anchor': 'bottom',
              'text-field': ['get', 'name'],
              'text-font': ['Open Sans Semibold', 'Arial Unicode MS Bold'],
              'text-anchor': 'top',
            },
          });
          this.map.moveLayer('markerLayer', 'pointLayer');
        }
      );

      this.map.addLayer({
        id: 'lineLayer',
        type: 'line',
        source: 'lineSource',
        paint: {
          'line-color': '#e55e5e',
          'line-width': 3,
        },
      });

      this.map.addLayer({
        id: 'polygonLayer',
        type: 'fill',
        source: 'polygonSource',
        paint: {
          'fill-color': '#e55e5e',
          'fill-opacity': 0.2,
        },
      });
    });

    this.map.on('dragend', this.storePosition.bind(this));
    this.map.on('zoomend', this.storeZoom.bind(this));
    this.map.on('click', this.addPoint.bind(this));
  }

  private storeZoom(): void {
    if (this.map == undefined) return;
    localStorage.setItem('mapZoom', this.map.getZoom().toString());
  }

  private storePosition(): void {
    if (this.map == undefined) return;
    localStorage.setItem('mapLatitude', this.map.getCenter().lat.toString());
    localStorage.setItem('mapLongitude', this.map.getCenter().lng.toString());
  }

  private restoreMapCamera(): void {
    if (this.map == undefined) return;

    let storedMapZoom = localStorage.getItem('mapZoom');
    let storedMapLatitude = localStorage.getItem('mapLatitude');
    let storedMapLongitude = localStorage.getItem('mapLongitude');

    if (storedMapZoom != null) {
      this.map.setZoom(Number.parseFloat(storedMapZoom));
    }

    if (storedMapLatitude != null && storedMapLongitude != null) {
      this.map.setCenter([
        Number.parseFloat(storedMapLongitude),
        Number.parseFloat(storedMapLatitude),
      ]);
    }
  }

  private addPoint(e: mapboxgl.MapMouseEvent): void {
    if (this.map == undefined) return;

    let clickedGeometries = this.map.queryRenderedFeatures(e.point);
    let clickedPoints = clickedGeometries.filter((geometry) =>
      geometry.source.toString().startsWith('point')
    );

    if (clickedPoints.length > 0) {
      clickedPoints.forEach((geometry) => {
        this.map?.removeLayer(geometry.source.toString());
        this.map?.removeSource(geometry.source.toString());
      });
      return;
    }

    this.map.addSource(`point${this.pointId}`, {
      type: 'geojson',
      data: {
        type: 'Feature',
        geometry: {
          type: 'Point',
          coordinates: [e.lngLat.lng, e.lngLat.lat],
        },
        properties: {
          name: 'point',
        },
      },
    });

    this.map.addLayer({
      id: `point${this.pointId}`,
      type: 'circle',
      source: `point${this.pointId}`,
      paint: {
        'circle-radius': {
          stops: [
            [1, 3],
            [15, 10],
          ],
        },
        'circle-color': '#e55e5e',
      },
    });

    this.pointId++;
  }

  protected GetShopsHeatMap(): void {
    if (this.map == undefined) return;
    if (this.map.getSource('shops')) {
      this.map.removeLayer('shops');
      this.map.removeSource('shops');
    }

    var bounds = this.map.getBounds();
    var p1 = bounds.getSouthWest();
    var p2 = bounds.getNorthEast();
    var options = { params: new HttpParams()
      .append('latPoint1', p1.lat.toString())
      .append('lonPoint1', p1.lng.toString())
      .append('latPoint2', p2.lat.toString())
      .append('lonPoint2', p2.lng.toString())
      .append('gridSquareInDeg', this.widthInDeg)
    }
    this.http.get<string>('api/osm/GetShopsHeatMap', options).subscribe(shops => {
      if (this.map == undefined) {
        return;
      }
console.log(shops);
      this.map.addSource('shops', {
        'type': 'geojson',
        'data': shops
        });

      this.map.addLayer({
        'id': 'shops',
        'type': 'fill',
        'source': 'shops',
        'layout': {},
        'paint': {
          'fill-color': [
          'interpolate',
          ['linear'],
          ['get', 'count'],
          0,
          '#F2F12D',
          5,
          '#EED322',
          7,
          '#E6B71E',
          10,
          '#DA9C20',
          25,
          '#CA8323',
          50,
          '#B86B25',
          75,
          '#A25626',
          100,
          '#8B4225',
          250,
          '#723122'
          ],
          'fill-opacity': [
            'interpolate',
            ['linear'],
            ['get', 'count'],
            0, 0, 5, 0.5
          ]
          }
        });

        console.log(this.map.getLayer('shops'))
        console.log(this.map.getSource('shops'))
    });
;
  }
}
