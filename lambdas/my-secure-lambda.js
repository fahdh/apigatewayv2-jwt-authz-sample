const jwtDecode = require('jwt-decode');
const responseHeaders = { "Access-Control-Allow-Origin": "*" };

exports.handler = async (event) => {
    // Token validation handled at api gateway.
    // Access token available -> event["headers"]["authorization"]
    console.log('event -> ', event);
    let headers = event["headers"];

    // Do further AuthZ checks here based on claims in the token e.g. role
    if (headers != null && headers.hasOwnProperty('authorization')) {
        var token = headers['authorization'].split(' ')[1];
        var decoded = jwtDecode(token);
        console.log('token decoded -> ', decoded);
        try {
            const response = {
                statusCode: 200,
                isBase64Encoded: false,
                body: "Authorization Successful!!",
                headers: responseHeaders
            };
            console.log('Success response...');

            return response;
        }
        catch (err) {
            console.error(err);
            return this.error400(JSON.stringify(err));
        }
    }
    else {
        console.error('Unauhthorized.');
        const errorResponse = {
            statusCode: 401,
            body: "Unauthorized.",
            headers: responseHeaders
        };

        return errorResponse;
    }
};

exports.error400 = (errorBody) => {
    const errorResponse = {
        statusCode: 400,
        statusText: 'Bad Request',
        body: errorBody,
        headers: responseHeaders
    };

    return errorResponse;
}
