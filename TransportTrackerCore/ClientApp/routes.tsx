import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import { MetraSchedule } from './components/MetraSchedule';
import { MetraActiveRoutes } from './components/MetraActiveRoutes';

export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/counter' component={ Counter } />
    <Route path='/fetchdata' component={FetchData} />
    <Route path='/metraactiveroutes' component={MetraActiveRoutes} />
    <Route path='/metraschedule' component={MetraSchedule} />
</Layout>;
