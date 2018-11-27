/// <binding BeforeBuild='Run - Development' AfterBuild='Run - Production' />
var path = require('path');

var extension = process.env.NODE_ENV == 'production' ? '.min' : '';
var usedevtool = process.env.NODE_ENV == 'production' ? false : 'cheap-module-eval-source-map';

module.exports = {
    mode: process.env.NODE_ENV,
    devtool: usedevtool,
    entry: './Content/js/index.js',
    output: {
        path: path.resolve(__dirname, 'wwwroot/js'),
        filename: 'versus' + extension + '.js'
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