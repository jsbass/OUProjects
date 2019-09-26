PromiseHttp = (function() {
    var cache = {};
    function get(url, config) {
        return makeRequest('GET', url, null, config);
    }

    function post(url, data, config) {
        return makeRequest('POST', url, data, config);
    }

    function deleteRequest(url, config) {
        return makeRequest('DELETE', url, null, config);
    }

    function makeRequest(method, url, data, config) {
        var hash = `${method}-${url}`;
        if (config && config.useCache) {
            if (cache.hasOwnProperty(hash)) {
                return cache[hash];
            }
        }
        var token = { cancel: function () { console.log('cancel not set'); } };
        var promise = new Promise(function(resolve, reject) {
            var req = new XMLHttpRequest();
            req.open(method, url);
            req.onload = function() {
                if (req.status == 200) {
                    resolve(req.response);
                } else {
                    reject(Error(req.statusText));
                }
            };

            req.onerror = function() {
                reject(Error('Network Error'));
            }

            token.cancel = function () {
                console.log('canceled');
                req.abort();
            }
            req.send(JSON.stringify(data));
        });
        var obj = {
            promise: promise,
            cancel: token.cancel
        }
        cache[hash] = obj;
        return obj;
    }

    return {
        'get': get,
        'post': post,
        'delete': deleteRequest
    }
})();