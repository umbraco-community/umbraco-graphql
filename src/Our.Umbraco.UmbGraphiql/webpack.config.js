var webpack = require("webpack");

module.exports = {
    output: {
        filename: "../Our.Umbraco.GraphQL/Resources/graphiql.js"
    },
    entry: {
        bundle: "./src/app.jsx"
    },
	
	externals: {
	},
	
	module: {
        loaders:  [
            {
                test: /\.jsx?$/,
                loader: 'babel-loader',
                exclude: /node_modules/,
                query: {
                    presets: ['react', 'es2015', 'stage-2']
                }
            }
        ]
    },
	
	// Prod:
	devtool: "eval"
}