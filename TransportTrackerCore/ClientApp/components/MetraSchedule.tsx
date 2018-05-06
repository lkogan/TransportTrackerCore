import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

interface FetchDataRouteState {
    routes: RoutesData[];
    loading: boolean;
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

export class MetraSchedule extends React.Component<RouteComponentProps<{}>, FetchDataRouteState> {

    constructor() {
        super();
         
        this.state = { routes: [], loading: true };
         
        fetch('api/Metra/GetRoutesData')
            .then(response => response.json() as Promise<RoutesData[]>)
            .then(data => {
                this.setState({ routes: data, loading: false });
            });
    }
     
    componentDidMount()
    {
        //this.loadData();
        //setInterval(this.loadData, 30000);
    }
     
    loadData()
    {
        try
        { 
            //fetch('api/Metra/GetRoutesData')
            //    .then(response => response.json() as Promise<RoutesData[]>)
            //    .then(data => {
            //        this.setState({ routes: data, loading: false });
            //    });


            //const res = await fetch('api/Metra/GetRoutesData');
            //const blocks = await res.json();
            //const dataPanelone = blocks.panelone;
            //const dataPaneltwo = blocks.paneltwo;

            //this.setState({
            //    panelone: dataPanelone,
            //    paneltwo: dataPaneltwo,
            //})





        } catch (e) {
            //console.log(e);
        }
    }

    public render() {
        //console.debug(this.state.routes);
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : MetraSchedule.renderForecastsTable(this.state.routes);

        return <div>
            <h1>Metra Active Routes</h1>
            <p>Fetching data from the server.</p>
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
