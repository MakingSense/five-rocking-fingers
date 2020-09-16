import * as React from 'react';
import { connect } from 'react-redux';

const Home = () => (
  <div>
    <h1>Let's Estimate some projects!!</h1>
  </div>
);

export default connect()(Home);
