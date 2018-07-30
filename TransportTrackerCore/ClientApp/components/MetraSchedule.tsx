import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { Button} from 'react-bootstrap';
import { ButtonToolbar } from 'react-bootstrap';
import { DropdownButton } from 'react-bootstrap';
import { MenuItem } from 'react-bootstrap';
import { ToggleButtonGroup } from 'react-bootstrap';
import { Label } from 'react-bootstrap';
import { ToggleButton } from 'react-bootstrap';
import Select from 'react-select';


interface FetchDataScheduleState {
    routes: StopOnTrip[];
    stations: StationObject[];
    loading: boolean;
    currentDateTime: string;
    fromStationSelected: string;
    toStationSelected: string;
}

interface StationObject
{
    value: string;
    label: string;
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
  

const options = [
    { value: 'WESTERNAVE', label: 'Western Avenue' },
    { value: 'HEALY', label: 'Healy' },
    { value: 'GRAYLAND', label: 'Grayland' }
];

var divStyle = { 
    margin: "20px" 
};

export class MetraSchedule extends React.Component<RouteComponentProps<{}>, FetchDataScheduleState> {

    constructor() {
        super();
         
        this.state = { routes: [], stations: [], loading: true, currentDateTime: '', fromStationSelected: null, toStationSelected: null}; 

        this.loadStations();
        this.loadData();
    }
     
    componentDidMount()
    {
        setInterval(() => {
            this.loadData();
        }, 30000)

        this.loadData();
    }

    loadStations()
    {
        try {
            fetch('api/Metra/GetStations')
                .then(response => response.json() as Promise<StationObject[]>)
                .then(data => {
                    this.setState({ stations: data });
                });

        } catch (e) {
            //console.log(e);
        }
    }

    loadData()
    {
        try
        { 
            fetch('api/Metra/GetScheduleData')
                .then(response => response.json() as Promise<StopOnTrip[]>)
                .then(data => {
                    this.setState({ routes: data, loading: false, currentDateTime: 'Updated: ' + new Date().toLocaleString() });
                });
             
        } catch (e) {
            //console.log(e);
        }
    }
 
    toStation_OnSelected = (toStationSelected) => {
        this.setState({ toStationSelected });
        console.log(`Option selected:`, toStationSelected);
    }

    fromStation_OnSelected = (fromStationSelected) => {
        this.setState({ fromStationSelected });
        console.log(`Option selected:`, fromStationSelected);
    }

    public render() {

        const { fromStationSelected } = this.state;
        const { toStationSelected } = this.state;

        //console.debug(this.state.routes);
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : MetraSchedule.renderForecastsTable(this.state.routes);

        return <div >
            <h1>Metra Scheduled Train Routes</h1>
            <p>
                <Label>Direction: </Label>
                <ToggleButtonGroup style={divStyle} type="radio" name="options" defaultValue={0}>
                    <ToggleButton value={0}>Outbound</ToggleButton>
                    <ToggleButton value={1}>Inbound</ToggleButton>
                </ToggleButtonGroup> 
            </p>
            <p>
                <Label>From: </Label>
                <Select  
                    value={fromStationSelected}
                    options={this.state.stations}
                    onChange={this.fromStation_OnSelected}
                />
            </p>
            <p>
                <Label>To: </Label>
                <Select
                    value={toStationSelected}
                    onChange={this.toStation_OnSelected}
                    options={options}
                />
            </p>
            <p>{this.state.currentDateTime}</p>
            {contents}
        </div>;
    }
     
    private static renderForecastsTable(routes: StopOnTrip[]) {
        return  <table className='table'>
                    <thead>
                        <tr>
                            <th>DepartingFrom</th>
                            <th>Departs At</th>
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
