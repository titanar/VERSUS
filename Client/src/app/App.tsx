import { Router } from '@reach/router';
import React, { lazy, Suspense } from 'react';
import { boundary, useError } from 'react-boundary';
import { Loader } from 'semantic-ui-react';

import { errors } from '../terms.en-us.json';
import { setTitle } from '../utilities/routing';
import { routes } from './routes';

const Frontend = lazy(() => import('./frontend/Frontend').then(module => ({ default: module.Frontend })));
const Error = lazy(() => import('./shared/Error').then(module => ({ default: module.Error })));

export const App = boundary(() => {
  const [error, info] = useError();

  if (error || info) {
    return <Error stack={`${error && error.stack}${info && info.componentStack}`} />;
  }

  setTitle();

  return (
    <Suspense fallback={<Loader active size='massive' />}>
      <Router>
        <Frontend path='*' />
        <Error path={routes.error} default message={errors.notFound} />
      </Router>
    </Suspense>
  );
});
