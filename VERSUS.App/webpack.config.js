var path = require('path');

module.exports = {
    mode: 'development',
    entry: './Content/js/index.js',
    output: {
        path: path.resolve(__dirname, 'wwwroot/js'),
        filename: 'versus.js'
    },
    module: {
        rules: [
            {
                test: /\.jsx?$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader'
                }
            }
        ]
    }
};
