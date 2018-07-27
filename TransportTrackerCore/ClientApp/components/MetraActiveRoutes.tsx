import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

interface FetchDataRouteState {
    routes: RoutesData[];
    loading: boolean;
    currentDateTime: string;
}


interface RoutesData {
    locationID: number;
    tripID: string;
    lat: string;
    long: string;
    description: string;
    imagePath: string;
    arrivalTime: string;
    tripURL: string;
    arrivesIn: string;
}

export class MetraActiveRoutes extends React.Component<RouteComponentProps<{}>, FetchDataRouteState> {

    constructor() {
        super();

        this.state = { routes: [], loading: true, currentDateTime: '' };

        this.loadData();
    }

    componentDidMount() {
        setInterval(() => {
            this.loadData();
        }, 30000)

        this.loadData();
    }

    loadData() {
        try {
            fetch('api/Metra/GetRoutesData')
                .then(response => response.json() as Promise<RoutesData[]>)
                .then(data => {
                    this.setState({ routes: data, loading: false, currentDateTime: 'Updated: ' + new Date().toLocaleString() });
                });

        } catch (e) {
            //console.log(e);
        }
    }

    public render() {
        //console.debug(this.state.routes);
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : MetraActiveRoutes.renderForecastsTable(this.state.routes);

        return <div>
            <h1>Metra Active Routes</h1>
            <p>{this.state.currentDateTime}</p>
            {contents}
        </div>;
    }

    private static renderForecastsTable(routes: RoutesData[]) {
        return <table className='table'>
            <thead>
                <tr>
                    <th>ArrivesIn</th>
                    <th>ArrivalTime</th>
                    <th>Description</th>
                    <th>TripID</th>
                    <th>TripURL</th>
                </tr>
            </thead>
            <tbody>
                {routes.map((route, index) =>
                    //<tr key={index}>
                    <tr key={route.locationID}>
                        <td>{route.arrivesIn}</td>
                        <td>{route.arrivalTime}</td>
                        <td>{route.description}</td>
                        <td>{route.tripID}</td>
                        <td>{route.tripURL}</td>
                    </tr>
                )}
            </tbody>
        </table>;
    }
}
