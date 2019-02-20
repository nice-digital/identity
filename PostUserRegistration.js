/**
@param {object} user - The user being created
@param {string} user.id - user id
@param {string} user.tenant - Auth0 tenant name
@param {string} user.username - user name
@param {string} user.email - email
@param {boolean} user.emailVerified - is e-mail verified?
@param {string} user.phoneNumber - phone number
@param {boolean} user.phoneNumberVerified - is phone number verified?
@param {object} user.user_metadata - user metadata
@param {object} user.app_metadata - application metadata
@param {object} context - Auth0 connection and other context info
@param {string} context.requestLanguage - language of the client agent
@param {object} context.connection - information about the Auth0 connection
@param {object} context.connection.id - connection id
@param {object} context.connection.name - connection name
@param {object} context.connection.tenant - connection tenant
@param {object} context.webtask - webtask context
@param {function} cb - function (error, response)
*/
module.exports = function (user, context, cb) {

  const https = require('https');
  
  const postData = JSON.stringify({
    'userId': 'Abc123'
  });

  const options = {
    hostname: 'npjyw5dg34.execute-api.eu-west-2.amazonaws.com',
    port: 443,
    path: '/default/users',
    method: 'POST',
    headers: {
      'x-api-key': 'HMC8IKPFNj2rwzgtz1NZLa4e8tXTlXrPvYN6cl90',
      'Content-Type': 'application/json'
    }
  };

  const req = https.request(options, (res) => {
    console.log(`STATUS: ${res.statusCode}`);
    console.log(`HEADERS: ${JSON.stringify(res.headers)}`);
  
    res.setEncoding('utf8');
    res.on('data', (chunk) => {
      console.log(`BODY: ${chunk}`);
    });
    res.on('end', () => {
      console.log('No more data in response.');
    });
  });

  req.on('error', (e) => {
  
    console.error(`problem with request: ${e.message}`);
  });

// write data to request body
  req.write(postData);
  req.end();

  
  cb();
};