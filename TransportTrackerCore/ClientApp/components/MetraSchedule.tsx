import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

interface FetchDataScheduleState {
    routes: StopOnTrip[];
    loading: boolean;
}


interface StopOnTrip {
    trip_id: string;
    arrival_time: string;
    departure_time: string;
    stop_id: string;
    stop_sequence: number;
    pickup_type: number;
    drop_off_type: number;
    center_boarding: number;
    south_boarding: number;
    bikes_allowed: number;
    notice: number;
} 

export class MetraSchedule extends React.Component<RouteComponentProps<{}>, FetchDataScheduleState> {

    constructor() {
        super();
         
        this.state = { routes: [], loading: true }; 

        this.loadData();
    }
     
    componentDidMount()
    {
        setInterval(() => {
            this.loadData();
        }, 30000)

        this.loadData();
        //setInterval(this.loadData, 30000);
    }
     
    loadData()
    {
        try
        { 
            fetch('api/Metra/GetScheduleData')
                .then(response => response.json() as Promise<StopOnTrip[]>)
                .then(data => {
                    this.setState({ routes: data, loading: false });
                });
             
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

    private static renderForecastsTable(routes: StopOnTrip[]) {
        return <table className='table'>
            <thead>
                <tr>
                    <th>DepartingFrom</th>
                    <th>ArrivalTime</th>
                    <th>TripID</th>
                </tr>
            </thead>
            <tbody>
                {routes.map((route, index) => 
                    <tr key={route.trip_id}>
                        <td>{route.stop_id}</td>
                        <td>{route.arrival_time}</td>
                        <td>{route.trip_id}</td>
                    </tr>
                )}
            </tbody>
        </table>;
    }
}
