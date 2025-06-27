'use strict'

const autocannon = require('autocannon')

const instance = autocannon({
  url: 'http://localhost:5062/test/feature-d',
  connections: 5000,
  pipelining: 1,
  duration: 30,
}, finishedBench)

autocannon.track(instance, {
  renderProgressBar: true,
  renderResultsTable: true,
  renderLatencyTable: true
});

function finishedBench(err, res) {
  if (err) {
    console.error('❌ Benchmark error:', err);
    return;
  }

  console.log('\n✅ Benchmark completed!');
  console.log('Summary:', {
    averageRequests: res.requests.average,
    averageLatency: res.latency.average + 'ms',
    errors: res.errors
  });

  console.log();
}
