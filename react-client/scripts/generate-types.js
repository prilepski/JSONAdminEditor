const { execSync } = require('child_process');

try {
  console.log('Generating TypeScript types from OpenAPI spec...');
  
  execSync('npx openapi-typescript http://localhost:5000/swagger/v1/swagger.json -o src/types/api.ts', {
    stdio: 'inherit'
  });
  
  console.log('Types generated successfully!');
} catch (error) {
  console.error('Error generating types:', error.message);
  process.exit(1);
}